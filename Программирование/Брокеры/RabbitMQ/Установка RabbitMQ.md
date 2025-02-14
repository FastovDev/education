
### Установка RabbitMQ

RabbitMQ можно установить на разных платформах: Windows, Linux и macOS. Вот основные шаги установки:

---

#### **1. Установка на Windows**

1. **Установите Erlang**:
    
    - RabbitMQ работает поверх Erlang, поэтому сначала нужно установить его.
    - Скачайте Erlang с официального сайта: Erlang Solutions.
    - Убедитесь, что путь к `erl.exe` добавлен в переменную среды `PATH`.
2. **Скачайте RabbitMQ**:
    
    - Скачайте RabbitMQ с официального сайта: RabbitMQ Download.
    - Установите RabbitMQ, следуя инструкциям установщика.
3. **Запустите RabbitMQ**:
    
    - После установки выполните команду:
        
        bash
        
        Копировать код
        
        `rabbitmq-service install rabbitmq-service start`
        
4. **Включите панель управления (Management Plugin)**:
    
    bash
    
    Копировать код
    
    `rabbitmq-plugins enable rabbitmq_management`
    
    После включения плагина вы сможете зайти в веб-интерфейс по адресу:  
    `http://localhost:15672` (логин/пароль по умолчанию: `guest/guest`).
    

---

#### **2. Установка на Ubuntu/Debian**

1. **Добавьте репозиторий RabbitMQ**:
    
    bash
    
    Копировать код
    
    `sudo apt update sudo apt install -y curl gnupg curl -fsSL https://packagecloud.io/rabbitmq/rabbitmq-server/gpgkey | sudo gpg --dearmor -o /usr/share/keyrings/rabbitmq-archive-keyring.gpg echo "deb [signed-by=/usr/share/keyrings/rabbitmq-archive-keyring.gpg] https://packagecloud.io/rabbitmq/rabbitmq-server/ubuntu focal main" | sudo tee /etc/apt/sources.list.d/rabbitmq.list`
    
2. **Установите Erlang**:
    
    bash
    
    Копировать код
    
    `sudo apt update sudo apt install -y erlang`
    
3. **Установите RabbitMQ**:
    
    bash
    
    Копировать код
    
    `sudo apt update sudo apt install -y rabbitmq-server`
    
4. **Запустите RabbitMQ**:
    
    bash
    
    Копировать код
    
    `sudo systemctl enable rabbitmq-server sudo systemctl start rabbitmq-server`
    
5. **Включите панель управления**:
    
    bash
    
    Копировать код
    
    `sudo rabbitmq-plugins enable rabbitmq_management`
    
    Веб-интерфейс будет доступен по адресу `http://localhost:15672`.
    

---

#### **3. Установка на macOS (через Homebrew)**

1. **Установите Homebrew**, если он ещё не установлен:
    
    bash
    
    Копировать код
    
    `/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"`
    
2. **Установите RabbitMQ**:
    
    bash
    
    Копировать код
    
    `brew install rabbitmq`
    
3. **Запустите RabbitMQ**:
    
    bash
    
    Копировать код
    
    `brew services start rabbitmq`
    
4. **Включите панель управления**:
    
    bash
    
    Копировать код
    
    `rabbitmq-plugins enable rabbitmq_management`
    
    Веб-интерфейс будет доступен по адресу `http://localhost:15672`.
    

---

### Основные настройки RabbitMQ

После установки RabbitMQ можно настроить с помощью конфигурационного файла или команд.

#### **1. Конфигурационный файл**

RabbitMQ использует файл конфигурации `rabbitmq.conf`, который находится в следующих местах:

- **Linux**: `/etc/rabbitmq/rabbitmq.conf`
- **Windows**: `%AppData%\RabbitMQ\rabbitmq.conf`

Примеры конфигураций:

ini

Копировать код

`# Установка порта listeners.tcp.default = 5672  # Логин/пароль по умолчанию default_user = admin default_pass = password  # Включение SSL (пример) ssl_options.cacertfile = /path/to/ca_certificate.pem ssl_options.certfile = /path/to/server_certificate.pem ssl_options.keyfile = /path/to/server_key.pem`

#### **2. Пользователи и права**

- Добавление пользователя:
    
    bash
    
    Копировать код
    
    `rabbitmqctl add_user username password`
    
- Назначение прав:
    
    bash
    
    Копировать код
    
    `rabbitmqctl set_permissions -p / username ".*" ".*" ".*"`
    

#### **3. Настройка плагинов**

- Включение плагина:
    
    bash
    
    Копировать код
    
    `rabbitmq-plugins enable <plugin_name>`
    
    Например:
    
    bash
    
    Копировать код
    
    `rabbitmq-plugins enable rabbitmq_management`
    

#### **4. Мониторинг**

- Плагин **Management** предоставляет веб-интерфейс.
- Для мониторинга можно также использовать Prometheus и Grafana.

#### **5. Логи**

- Путь к логам можно настроить в `rabbitmq.conf`:
    
    ini
    
    Копировать код
    
    `log.file.level = info log.file.location = /var/log/rabbitmq/rabbitmq.log`