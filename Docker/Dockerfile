FROM ubuntu:16.04

VOLUME [ "/data/unturned" ]

EXPOSE 27016-27017/udp

ADD data /data

RUN dpkg --add-architecture i386
RUN \
    apt-get update && \
    apt-get -y install \
    mono-runtime mono-reference-assemblies-2.0 \
    libc6:i386 libgl1-mesa-glx:i386 libxcursor1:i386 libxrandr2:i386 \
    libc6-dev-i386 libgcc-4.8-dev:i386 \
        unzip wget

CMD [ "/bin/bash", "-c", "/data/main.sh" ]