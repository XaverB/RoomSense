# Use the official PostgreSQL base image
FROM postgres:latest

# Set environment variables for the database
ENV POSTGRES_USER roomsense-service
ENV POSTGRES_PASSWORD iot
ENV POSTGRES_DB roomsense

# Copy any initialization scripts (if needed)
# COPY init.sql /docker-entrypoint-initdb.d/

# Expose the default PostgreSQL port
EXPOSE 5432

# Run any additional commands if needed
# RUN ...

# Set the entry point to start PostgreSQL
CMD ["postgres"]