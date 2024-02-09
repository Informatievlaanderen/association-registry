FROM postgres:14

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

# Install the AWS CLI
RUN apt-get update
RUN apt-get install python3-pip pipx -y

RUN pipx install awscli

COPY .aws/ /app
WORKDIR /app

ENTRYPOINT ["./init.sh"]