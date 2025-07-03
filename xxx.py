#!/usr/bin/env python3
"""
Script to remove PackageReference elements from .csproj files that don't have
corresponding PackageVersion entries in Directory.Packages.props
"""
import os
import xml.etree.ElementTree as ET
from pathlib import Path
import argparse
import shutil
from datetime import datetime
def get_package_versions_from_directory_props(directory_props_path):
    """
    Extract all PackageVersion Include values from Directory.Packages.props
    """
    if not os.path.exists(directory_props_path):
        print(f"ERROR: Directory.Packages.props not found at {directory_props_path}")
        return set()
    try:
        tree = ET.parse(directory_props_path)
        root = tree.getroot()
        package_versions = set()
        # Find all PackageVersion elements
        for package_version in root.findall('.//PackageVersion'):
            include_attr = package_version.get('Include')
            if include_attr:
                package_versions.add(include_attr)
        print(f"Found {len(package_versions)} PackageVersion entries in Directory.Packages.props")
        return package_versions
    except ET.ParseError as e:
        print(f"ERROR: Failed to parse Directory.Packages.props: {e}")
        return set()
def get_package_references_from_csproj(csproj_path):
    """
    Extract all PackageReference elements from a .csproj file
    Returns list of (element, include_value) tuples
    """
    try:
        tree = ET.parse(csproj_path)
        root = tree.getroot()
        package_references = []
        # Find all PackageReference elements
        for package_ref in root.findall('.//PackageReference'):
            include_attr = package_ref.get('Include')
            if include_attr:
                package_references.append((package_ref, include_attr))
        return tree, package_references
    except ET.ParseError as e:
        print(f"ERROR: Failed to parse {csproj_path}: {e}")
        return None, []
def backup_file(file_path, backup_dir):
    """
    Create a backup of the file
    """
    if not os.path.exists(backup_dir):
        os.makedirs(backup_dir)
    backup_path = os.path.join(backup_dir, os.path.basename(file_path))
    counter = 1
    while os.path.exists(backup_path):
        name, ext = os.path.splitext(os.path.basename(file_path))
        backup_path = os.path.join(backup_dir, f"{name}_{counter}{ext}")
        counter += 1
    shutil.copy2(file_path, backup_path)
    return backup_path
def remove_orphaned_package_references(root_dir, directory_props_path, dry_run=False, create_backup=True):
    """
    Remove PackageReference elements that don't have corresponding PackageVersion entries
    """
    # Get all package versions from Directory.Packages.props
    package_versions = get_package_versions_from_directory_props(directory_props_path)
    if not package_versions:
        print("No package versions found. Exiting.")
        return
    # Find all .csproj files recursively
    csproj_files = []
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file.endswith('.csproj'):
                csproj_files.append(os.path.join(root, file))
    print(f"Found {len(csproj_files)} .csproj files")
    # Create backup directory if needed
    backup_dir = None
    if create_backup and not dry_run:
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        backup_dir = f"backup_csproj_{timestamp}"
        print(f"Backups will be created in: {backup_dir}")
    total_removed = 0
    modified_files = []
    # Process each .csproj file
    for csproj_path in csproj_files:
        tree, package_references = get_package_references_from_csproj(csproj_path)
        if not tree or not package_references:
            continue
        # Find PackageReference elements to remove
        to_remove = []
        for element, include_value in package_references:
            if include_value not in package_versions:
                to_remove.append((element, include_value))
        if not to_remove:
            continue
        print(f"\n{csproj_path}:")
        print(f"  Found {len(package_references)} PackageReference elements")
        print(f"  Will remove {len(to_remove)} orphaned references:")
        for element, include_value in to_remove:
            print(f"    - {include_value}")
        if not dry_run:
            # Create backup if requested
            if create_backup:
                backup_path = backup_file(csproj_path, backup_dir)
                print(f"    Backup created: {backup_path}")
            # Remove orphaned elements
            root = tree.getroot()
            for element, include_value in to_remove:
                # Safe way to remove the element by finding its parent manually
                for parent in root.iter():
                    if element in list(parent):
                        parent.remove(element)
                        break
            # Write the modified file
            tree.write(csproj_path, encoding='utf-8', xml_declaration=True)
            print(f"    Modified: {csproj_path}")
            modified_files.append(csproj_path)
        total_removed += len(to_remove)
    # Summary
    print(f"\n{'DRY RUN - ' if dry_run else ''}SUMMARY:")
    print(f"  Total .csproj files processed: {len(csproj_files)}")
    print(f"  Total PackageReference elements {'would be ' if dry_run else ''}removed: {total_removed}")
    print(f"  Files {'would be ' if dry_run else ''}modified: {len(modified_files)}")
    if dry_run:
        print("\nThis was a dry run. No files were modified.")
        print("Run with --execute to actually remove the PackageReference elements.")
def main():
    parser = argparse.ArgumentParser(
        description="Remove orphaned PackageReference elements from .csproj files"
    )
    parser.add_argument(
        "--root-dir",
        default=".",
        help="Root directory to search for .csproj files (default: current directory)"
    )
    parser.add_argument(
        "--directory-props",
        default="Directory.Packages.props",
        help="Path to Directory.Packages.props file (default: Directory.Packages.props)"
    )
    parser.add_argument(
        "--execute",
        action="store_true",
        help="Actually modify files (default: dry run)"
    )
    parser.add_argument(
        "--no-backup",
        action="store_true",
        help="Don't create backup files"
    )
    args = parser.parse_args()
    # Convert to absolute paths
    root_dir = os.path.abspath(args.root_dir)
    directory_props_path = os.path.abspath(args.directory_props)
    print(f"Root directory: {root_dir}")
    print(f"Directory.Packages.props: {directory_props_path}")
    print(f"Mode: {'EXECUTE' if args.execute else 'DRY RUN'}")
    print(f"Backup: {'No' if args.no_backup else 'Yes'}")
    print("-" * 60)
    # Validate inputs
    if not os.path.exists(root_dir):
        print(f"ERROR: Root directory does not exist: {root_dir}")
        return 1
    if not os.path.exists(directory_props_path):
        print(f"ERROR: Directory.Packages.props file does not exist: {directory_props_path}")
        return 1
    # Run the cleanup
    remove_orphaned_package_references(
        root_dir=root_dir,
        directory_props_path=directory_props_path,
        dry_run=not args.execute,
        create_backup=not args.no_backup
    )
    return 0
if __name__ == "__main__":
    exit(main())







