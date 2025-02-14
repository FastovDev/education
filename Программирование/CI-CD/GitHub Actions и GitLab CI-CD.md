
CI/CD (Continuous Integration / Continuous Deployment) — это практика автоматизации сборки, тестирования и развертывания кода.

### **1.1. Основные этапы CI/CD**

1. **Continuous Integration (CI)** — автоматическая проверка кода после коммита:  
    ✅ Запуск юнит-тестов  
    ✅ Статический анализ кода  
    ✅ Сборка проекта
    
2. **Continuous Delivery (CD)** — автоматическая подготовка к развертыванию:  
    ✅ Сборка артефактов  
    ✅ Деплой на тестовый сервер
    
3. **Continuous Deployment (CD)** — автоматическое развертывание в продакшен:  
    ✅ Размещение приложения на боевом сервере  
    ✅ Мониторинг и откат в случае ошибки
    

---

## **2. GitHub Actions: автоматизация в GitHub**

GitHub Actions — это встроенный механизм CI/CD в GitHub.

### **2.1. Основные концепции GitHub Actions**

🔹 **Workflow** — файл, описывающий процесс CI/CD  
🔹 **Job** — набор шагов (steps), выполняемых на виртуальной машине  
🔹 **Step** — отдельный шаг (например, сборка или тест)  
🔹 **Runner** — сервер, который выполняет workflow

### **2.2. Создание первого workflow**

Workflow-файл создаётся в `.github/workflows/ci.yml`.

Пример: запуск тестов при каждом коммите в `main`:

```yaml
name: CI Pipeline

on:
  push:
    branches:
      - main
  pull_request:
    branches:
      - main

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout репозиторий
        uses: actions/checkout@v3

      - name: Установка .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0'

      - name: Сборка проекта
        run: dotnet build --configuration Release

      - name: Запуск тестов
        run: dotnet test

```

**Разбор:**

- `on: push` — запуск на каждый коммит в `main`
- `runs-on: ubuntu-latest` — выполнение на сервере с Ubuntu
- `dotnet build` — сборка проекта
- `dotnet test` — запуск тестов

---

### **2.3. Деплой в Docker с GitHub Actions**

Пример деплоя Docker-образа в **Docker Hub**:

```yaml
name: Build and Push Docker Image

on:
  push:
    branches:
      - main

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout репозиторий
        uses: actions/checkout@v3

      - name: Логин в Docker Hub
        run: echo "${{ secrets.DOCKER_PASSWORD }}" | docker login -u "${{ secrets.DOCKER_USERNAME }}" --password-stdin

      - name: Сборка Docker-образа
        run: docker build -t myapp:latest .

      - name: Отправка образа в Docker Hub
        run: docker push myapp:latest

```

Здесь используются **GitHub Secrets** (`DOCKER_USERNAME`, `DOCKER_PASSWORD`) для безопасной авторизации.

---

## **3. GitLab CI/CD: автоматизация в GitLab**

GitLab CI/CD использует файл `.gitlab-ci.yml` для определения pipeline.

### **3.1. Основные концепции GitLab CI/CD**

🔹 **Pipeline** — процесс CI/CD (состоит из стадий)  
🔹 **Job** — конкретная задача (например, тестирование)  
🔹 **Stage** — группа задач (build, test, deploy)  
🔹 **Runner** — сервер, выполняющий задачи

---

### **3.2. Создание `.gitlab-ci.yml`**

Простой pipeline, выполняющий сборку и тестирование `.NET`-приложения:

```yaml
stages:
  - build
  - test

variables:
  DOTNET_VERSION: "8.0"

build:
  stage: build
  image: mcr.microsoft.com/dotnet/sdk:8.0
  script:
    - dotnet restore
    - dotnet build --configuration Release

test:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:8.0
  script:
    - dotnet test

```

Здесь используются:

- `stages` — два этапа: `build` и `test`
- `image` — Docker-образ с .NET SDK
- `script` — команды, выполняемые в контейнере

---

### **3.3. Деплой в Docker с GitLab CI/CD**

```yaml
stages:
  - build
  - deploy

variables:
  IMAGE_NAME: registry.gitlab.com/myproject/myapp

build:
  stage: build
  image: docker:latest
  services:
    - docker:dind
  script:
    - docker build -t $IMAGE_NAME:latest .
    - docker login -u "$CI_REGISTRY_USER" -p "$CI_REGISTRY_PASSWORD" $CI_REGISTRY
    - docker push $IMAGE_NAME:latest

```

Здесь:

- `docker:dind` — запуск Docker внутри CI/CD
- `$CI_REGISTRY_USER` и `$CI_REGISTRY_PASSWORD` — переменные для аутентификации

---

## **4. Сравнение GitHub Actions и GitLab CI/CD**

|Функция|GitHub Actions|GitLab CI/CD|
|---|---|---|
|Язык конфигурации|YAML|YAML|
|Запуск по webhook|✅|✅|
|Поддержка Docker|✅|✅|
|Secrets|✅|✅|
|Бесплатные минуты|Ограничены|Ограничены|
|Встроенный registry|❌ (только через Docker Hub)|✅ (GitLab Container Registry)|

### **Когда использовать?**

- **GitHub Actions** — если проект в **GitHub**, простая настройка
- **GitLab CI/CD** — если работа в **GitLab**, мощный registry

---

## **5. Полезные команды для отладки CI/CD**

🔹 **GitHub Actions:**

- Просмотреть логи: **Actions → Workflow → Job Details**
- Перезапустить workflow: **Actions → Re-run jobs**

🔹 **GitLab CI/CD:**

- Просмотреть логи: **CI/CD → Pipelines → Job**
- Перезапустить pipeline:

```sh
git commit --allow-empty -m "Trigger CI"
git push

```

---

## **6. Вывод**

CI/CD **автоматизирует** процесс разработки, сокращает ошибки и ускоряет деплой.

🔹 **GitHub Actions** → удобен для проектов на GitHub  
🔹 **GitLab CI/CD** → лучше для работы с приватными репозиториями и Docker Registry

> Рекомендуется освоить **GitHub Actions** и **GitLab CI/CD** для уверенного владения DevOps-практиками. 🚀