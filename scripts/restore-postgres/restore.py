import os
import pandas as pd
from sqlalchemy import create_engine
import logging

# Database connection settings
db_username = 'username'
db_password = 'password'
db_host = '127.0.0.1'
db_port = '5432'
db_name = 'verenigingsregister'

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')


# PostgreSQL engine
engine = create_engine(f'postgresql://{db_username}:{db_password}@{db_host}:{db_port}/{db_name}')

# Directory containing Parquet files
parquet_dir = '/path/to/verenigingsregister'

def format_table_name(path):
    parts = path.split(os.sep)
    if len(parts) > 1:
        # Extracts the part after 'public.'
        schema_table = parts[-2]
        if schema_table.startswith('public.'):
            return schema_table.split('.')[1]
    return None

# Function to load a Parquet file into PostgreSQL
def load_parquet(file_path, table_name):
    df = pd.read_parquet(file_path)
    df.to_sql(table_name, engine, if_exists='append', index=False)

# Loop through directories and files
for root, dirs, files in os.walk(parquet_dir):
    for file in files:
        if file.endswith('.gz.parquet'):
            file_path = os.path.join(root, file)
            # Extract and format table name
            table_name = format_table_name(root)
            if table_name:
                load_parquet(file_path, table_name)

print("Data import complete.")