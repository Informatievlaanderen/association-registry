// Script to generate diagram images using Mermaid CLI
// Run: npm install -g @mermaid-js/mermaid-cli
// Then: node generate-diagrams.js

const fs = require('fs');
const { execSync } = require('child_process');

// Read the markdown file
const content = fs.readFileSync('REFACTORING_SUMMARY.md', 'utf8');

// Extract Mermaid diagrams
const mermaidRegex = /```mermaid\n([\s\S]*?)\n```/g;
let match;
let diagramIndex = 1;

while ((match = mermaidRegex.exec(content)) !== null) {
    const diagramContent = match[1];
    const filename = `diagram-${diagramIndex}`;
    
    // Write diagram to temp file
    fs.writeFileSync(`${filename}.mmd`, diagramContent);
    
    // Generate PNG
    try {
        execSync(`mmdc -i ${filename}.mmd -o ${filename}.png -t dark -b white`);
        console.log(`Generated ${filename}.png`);
    } catch (error) {
        console.error(`Failed to generate ${filename}.png:`, error.message);
    }
    
    // Clean up temp file
    fs.unlinkSync(`${filename}.mmd`);
    
    diagramIndex++;
}