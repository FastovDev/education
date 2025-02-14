version: "3.5"
services:
	web-server: - имя любое для описания контейнера
		image: nginx:stable - версия образа
		container_name: mynginx  - имя контейнера
		volumes: - расположение и соотношение томов
			- '/opt/web/html:/var/www/html'
			- '/opt/web/pics:/var/www/pictures'
		environment: - добавляем переменные окружения
			- 'NGINX_HOST=web.romero.de'
			- 'NGINX_PORT=80'
		ports: - порты контейнера
			- '80:80'
			- '443:443'
		restart: unless-stopped  /always/no/on-failure
		command: --transaction-isolation=READ-COMMITED --binlog-format=ROW (взято из примера с базой данных - команда которая будет запущена при запуске контейнера)
		depends_on: - запуск контейнера после запуска других указанных в зависимости контейнеров 
		    - container_name
		 networks: 
			-appnet
			-internet
networks:
	appnet:
		driver: bridge
		name: webnet

volumes:
	nginx-config: - создание именных томов

docker-compose up (-d) - old run
docker compose up (-d) - new run

вместо image можно указывать build ., соберет образ из докерфайла

docker-compose logs -f
docker-compose stop
docker-compose down

Если не указывать сеть, то создается сеть с именем контейнера, web-server-default