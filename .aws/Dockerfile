FROM public.ecr.aws/aws-cli/aws-cli:2.15.2

RUN yum install -y bash

WORKDIR /app

COPY scripts/ecs-desired-count.sh .

RUN chmod +x ./ecs-desired-count.sh

ENTRYPOINT ["./ecs-desired-count.sh"]