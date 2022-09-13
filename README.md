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
There is an acompanying console application; `UTB.Console`. It is not included
in the dockerized build but can be used to run the data fetching code and to
populate the database. It will also use the .env file to produce the connection
string so it can be configured to connect to wherever the database is.

## Running the containerised application
Make sure that an .env file exists and it is properly setup to match the
postgres database. Then bring the project up with `docker-compose up -d` or
`docker-compose up -d --build` to force everything to be rebuilt.

