FROM ubuntu:18.04

RUN apt-get -y update && \
    apt-get -y install g++ \
                       gcc \
                       git \
                       make \
                       zip
COPY . /bannertool
WORKDIR /bannertool
CMD ["make"]
