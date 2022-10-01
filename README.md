# Database configuration
You can copy `sample.env` to `.env` and change the values in there to configure
what user/password postgres will use to configure itself. Those variables have
no effect if the database is already initialized.

Everything needed by the postgres container will be stored in the `postgres`
folder. However due to how permissions work the folder will appear as empty
unless you navigate to it as root. If you ever need to re-initialize the
database you can just delete the contents of the `postgres` folder and the
container will re-initialize on the next `docker-compose up`.

## Database Migrations
Migrations need to be run on the `UTB.Data` project. The .env file will be used
to generate the connection string (same as if we were running the API).

## Console application
There is an accompanying console application; `UTB.Console`. It is not included
in the dockerized build but can be used to run the data fetching code and to
populate the database. It will also use the .env file to produce the connection
string so it can be configured to connect to wherever the database is.

## Running the containerized application
Make sure that an .env file exists and it is properly setup to match the
postgres database. Then bring the project up with `docker-compose up -d` or
`docker-compose up -d --build` to force everything to be rebuilt.

# Notes on Kubernetes deployment

The `kubernetes` directory includes _most_ of the manifests you will need to
deploy on GKE. There are two secrets missing, `postgres-secret` and `sa-secret`.
The first one can be generated from an .env file similar to what is mentioned in
the previous section using 
```sh
kubectl create secret generic postgres-secret --from-env-file=<PATH_TO_FILE>
```
The later needs to be generated from a service account with 
```sh
kubectl create secret generic sa-secret \
--from-file=service_account.json=<PATH_TO_SERVICE_ACCOUNT.json>
```

The k8s deployment must use cloud_sql_proxy to connect to the database, and the
host in this case must always be `127.0.0.1` or `localhost`.


