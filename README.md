# Database configuration
You can copy `sample.env` to `.env` and change the values in there to configure
what user/password postgres will use to configure itself. Those variables have
no effect if the database is already initialized.

Everything needed by the postgres container will be stored in the `database`
folder. However due to how permissions work the folder will appear as empty
unless you navigate to it as root. If you ever need to re-initialize the
database you can just delete the contents of the `database` folder and the
container will re-initialize on the next `docker-compose up`.