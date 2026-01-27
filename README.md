## Комментарии к решению

## Задание 1

Выделил 4 домена.
#### Пользователи
Управление доступом, история, профили

#### Монетизация
Подписки, тарифы, билинг и пр

#### Контент
Управление доступом к контенту

#### Интеграции
Взаимодействие с внешними системами

Диаграмма С4 ([svg](schemas/schema_tobe.svg ), [puml](schemas/schema.puml ))


## Задание 2

### 1. Proxy
Сервис реализован на .NET (C#)<br>
Не очень понял как реализовать маршрутизацию с заданным набором настроек (MOVIES_MIGRATION_PERCENT и пр.). Реализовал как сам вижу.<br>
Каждому префиксу соотвествует пара URL-ов и процент распределения трафика между MigrationUrl (столько сколько указано в MigrationPercent) и OldUrl (остаток).<br><br>
Реализацию см в RouteSelector.cs
Конфиг из appsettings.json

```json
  "Proxy": {
    "Routes": [
        {
         "PathPrefix": "/api/movies",
         "MigrationPercent": 50,
         "MigrationUrl": "http://localhost:8081/api/movies",
         "OldUrl": "http://localhost:8080/api/movies"
        },
        {
         "PathPrefix": "/api/users",
         "MigrationPercent": 0,
         "MigrationUrl": "",
         "OldUrl": "http://localhost:8080/api/users"
        }
     ]
  }
```

### 2. Kafka
Сервис реализован на .NET (C#)

Необходимые тесты для проверки этого API вызываются при запуске npm run test:local из папки tests/postman
Приложите скриншот тестов и скриншот состояния топиков Kafka http://localhost:8090 <br>
- Тесты ([screen1](screens/test_s1.png ), [screen2](screens/test_s2.png ), [screen3](screens/test_s3.png ))
- Kafka ([movie-events](screens/kafka_s1.png ), [payment-events](screens/kafka_s2.png ), [user-events](screens/kafka_s3.png ))

## Задание 3
### CI/CD
### Proxy в Kubernetes

#### Шаг 1
Локально для отладки изспользовал Kind + Lens.<br>
```yaml
kind: Cluster
apiVersion: kind.x-k8s.io/v1alpha4
nodes:
- role: control-plane
  extraPortMappings:
    - containerPort: 80
      hostPort: 80
      protocol: TCP
    - containerPort: 443
      hostPort: 443
      protocol: TCP
```
```bash
kind create cluster --name cinema-local --config ./kind/kind-config.yaml
```
#### Шаг 3
- [скриншота вывода при вызове https://cinemaabyss.example.com/api/movies](screens/task3_s1.png )
- [скриншот вывода event-service ](screens/task3_s2.png )

## Задание 4
Порядок приседаний для разворачивания кластера
```
1. kind create cluster --name cinema-local --config ./kind/kind-config.yaml
2. kubectl apply -f namespace.yaml
3. kubectl config use-context kind-cinema-local
4. kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml
5. helm install cinemaabyss ./helm -n cinemaabyss
```
- [Скриншот развертывания Helm](screens/helm_status.png )
- [Вывод https://cinemaabyss.example.com/api/movies](screens/helm_status2.png )

# Задание 5
```
kubectl exec -n cinemaabyss -it deploy/fortio -- fortio load -c 50 -qps 0 -n 500 http://movies-service.cinemaabyss.svc.cluster.local:8081/api/movies
```
[скриншот работы команды](screens/fortio1.png )<br><br>
```
kubectl exec -n cinemaabyss -it deploy/fortio -- fortio load -c 50 -qps 0 -n 500 http://monolith.cinemaabyss.svc.cluster.local:8080/api/users
```
[скриншот работы команды](screens/fortio2.png )

```
