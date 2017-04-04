FROM ubuntu

EXPOSE 27016/udp
EXPOSE 27017/udp
VOLUME ["/home/rocket/unturned/Servers/"]

RUN useradd -ms /bin/bash rocket
RUN apt-get update && apt-get install -y apt-utils cron ca-certificates lib32gcc1 unzip net-tools lib32stdc++6 lib32z1 lib32z1-dev curl wget screen tmux libmono-cil-dev mono-runtime

RUN mkdir -p /home/rocket/steamcmd && curl -s http://media.steampowered.com/installer/steamcmd_linux.tar.gz | tar -v -C /home/rocket/steamcmd -zx
RUN mkdir -p /home/rocket/unturned

ADD bash/start.sh /home/rocket/start.sh
RUN chmod a+x /home/rocket/start.sh
RUN (crontab -l ; echo "* * * * * /home/rocket/steamcmd/start.sh rocket") | sort - | uniq - | crontab -

ADD bash/update.sh /home/rocket/update.sh
RUN chmod a+x /home/rocket/update.sh
RUN (crontab -l ; echo "@daily /home/rocket/steamcmd/update.sh") | sort - | uniq - | crontab -

ADD credentials/STEAM_USERNAME /root/.steam_user
ADD credentials/STEAM_PASSWORD /root/.steam_pass

ONBUILD USER root
ONBUILD run /home/rocket/update.sh
