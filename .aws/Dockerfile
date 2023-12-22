FROM public.ecr.aws/aws-cli/aws-cli:2.15.2

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

COPY .aws/ /app
WORKDIR /app

RUN yum install -y bash curl

ENTRYPOINT ["./init.sh"]