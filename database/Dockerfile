# Use the official PostgreSQL base image
FROM postgres:latest

# Set environment variables for the database
ENV POSTGRES_USER myuser
ENV POSTGRES_PASSWORD mypassword
ENV POSTGRES_DB mydb

# Copy any initialization scripts (if needed)
# COPY init.sql /docker-entrypoint-initdb.d/

# Expose the default PostgreSQL port
EXPOSE 5432

# Run any additional commands if needed
# RUN ...

# Set the entry point to start PostgreSQL
CMD ["postgres"]