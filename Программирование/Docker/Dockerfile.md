FROM imageName - base image 
LABEL anyinformation - description image with inspect command
RUN anycommand - run commands for image or filesystem in container
WORKDIR pathfrom . - choose work dir
COPY pathfrom pathto - copying files in workdir or other place
ENV NAME="value" - type env for container
EXPOSE portvalue - open ports for container, information context
CMD - command after run container
ENTRYPOINT - unchanged command after run container

Container based on other image, this can be some basic image or other image
docker build . - create image
docker tag imageid mydocker:v01 - set name and tag for exist image
docker build -t myimage:v01 .
docker image inspect nameimage - see image inner
-P большая назначает случайный порт на вход в expose контейнера