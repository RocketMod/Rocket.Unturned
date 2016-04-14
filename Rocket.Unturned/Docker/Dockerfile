FROM ubuntu

EXPOSE 27016/udp 27017/udp
VOLUME ["/home/rocket/unturned/Servers/"]

RUN useradd -ms /bin/bash rocket
RUN apt-get update && apt-get install -y ca-certificates lib32gcc1 unzip net-tools lib32stdc++6 lib32z1 lib32z1-dev curl screen libmono2.0-cil mono-runtime

RUN mkdir -p /home/rocket/steamcmd  && curl -s http://media.steampowered.com/installer/steamcmd_linux.tar.gz | tar -v -C /home/rocket/steamcmd -zx
RUN mkdir -p /home/rocket/unturned

ONBUILD USER root
ONBUILD ADD update.sh /home/rocket/steamcmd/update.sh
ONBUILD RUN chmod 777 /home/rocket/steamcmd/update.sh
ONBUILD RUN chmod 777 (crontab -l ; echo "* * * * * /home/rocket/steamcmd/update.sh") | sort - | uniq - | crontab -

ONBUILD ADD start.sh /home/rocket/unturned/start.sh
ONBUILD RUN chmod 777 /home/rocket/unturned/start.sh
ONBUILD RUN chmod 777 (crontab -l ; echo "* * * * * /home/rocket/unturned/start.sh") | sort - | uniq - | crontab -