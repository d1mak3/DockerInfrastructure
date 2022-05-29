# DockerInfrastructure

____

## Стек технологий
В качестве базы данных выбрана [MySQL](https://www.mysql.com). Сервер с отправкой и реализован на [сервера Asp .NET 6.0](https://github.com/d1mak3/DockerInfrastructure/tree/master/BrokerServer) и играет роль *producer* в брокере. В качестве брокера сообщений был выбран [RabbitMQ](https://www.rabbitmq.com). Роль *consumer* исполняет [консольное приложение .NET 6.0](https://github.com/d1mak3/DockerInfrastructure/tree/master/MessagesServer). Для кэширования запросов к базе данных выбран [redis]("https://redis.io").

____

## Брокер
Сообщения в брокер генерируются [продюсером](https://github.com/d1mak3/DockerInfrastructure/tree/master/BrokerServer), который является web-приложением. Он имеет методы POST и GET.

Метод POST имеет путь api/producer и принимает на вход JSON строку, где указываются поля "Id", "Sender" и "Content":
![Alt-текст](https://disk.yandex.ru/i/FVTHTBH-g8uIhQ "Пример POST запроса")

Метод GET имеет путь api/producer и ничего не принимает на вход:
![Alt-текст](https://disk.yandex.ru/i/a2tOYeFINo-2Fg "Пример GET запроса")

Сообщения принимаются [консьюмером](https://github.com/d1mak3/DockerInfrastructure/tree/master/MessagesServer). В методе `public void GetQueryFromTheQueue()` класса `RabbitController` происходит парсинг команды, посланной продюссером, после чего выполняется операция, указанная в команде.

____

## Подключение к базе данных
Для подключения к бд я создал интерфейс `IDatabaseController`, который реализуется классом `MySqlDatabaseController` для подключения к MySQL. За сохранение сообщений отвечает метод `public bool SaveMessage(Models.Message messageToSave)`, а за получение истории сообщений — `public List<Models.Message> GetMessages()`. 

____

## Кэширование
Для кэширования (а точнее подключения к redis) я создал интерфейс `ICacheController`, который реализуется классом `RedisController` для подключения к redis. За поиск значения по ключу отвечает метод `public string GetValueByKey(string key)`, а за изменение/создания значения используется `public bool SetValue(string key, string value)`.

____