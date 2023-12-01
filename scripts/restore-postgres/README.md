# Restore backup from S3

## Copy file from S3 to local env

```
aws s3 cp s3://<bucketname>/<exported-data-directory> </local/path> --recursive --profile <AWS_PROFILE>
```

## Install dependencies

```
pip install pandas pyarrow psycopg2 sqlalchemy
```

## Create database through MartenDb

Eg: start the services once on your local dev machine, stop them after db creation.

## Run restore.py

```
python restore.py
```
