# DockerInfrastructure

____

## Стек технологий
В качестве базы данных выбрана [MySQL](https://www.mysql.com). Сервер с отправкой и реализован на [сервера Asp .NET 6.0](https://github.com/d1mak3/DockerInfrastructure/tree/master/BrokerServer) и играет роль *producer* в брокере. В качестве брокера сообщений был выбран [RabbitMQ](https://www.rabbitmq.com). Роль *consumer* исполняет [консольное приложение .NET 6.0](https://github.com/d1mak3/DockerInfrastructure/tree/master/MessagesServer). Для кэширования запросов к базе данных выбран [redis]("https://redis.io").

____

## Брокер
Сообщения в брокер генерируются [продюсером](https://github.com/d1mak3/DockerInfrastructure/tree/master/BrokerServer), который является web-приложением. Он имеет методы POST и GET.

Метод POST имеет путь api/producer и принимает на вход JSON строку, где указываются поля "Id", "Sender" и "Content":
![Alt-текст](https://disk.yandex.ru/client/disk/ScreenshotsForDockerInfrastructureRepo?idApp=client&dialog=slider&idDialog=%2Fdisk%2FScreenshotsForDockerInfrastructureRepo%2Fизображение_2022-05-29_065508592.png "Пример POST запроса")

Метод GET имеет путь api/producer и ничего не принимает на вход:
![Alt-текст](https://downloader.disk.yandex.ru/preview/9d4b7303ea3d8b5a9d2554d867671352dffa9b09dedc4aec3793c54d0a618516/62932ba7/rYc1beGpATDAO1RPp__6QDIir0q5LzUoc66cJL2P2BsttlJJS4A9jPWnhC777rynkuw_jvEaag6nAmKatXGdvA%3D%3D?uid=0&filename=%D0%B8%D0%B7%D0%BE%D0%B1%D1%80%D0%B0%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5_2022-05-29_065750119.png&disposition=inline&hash=&limit=0&content_type=image%2Fpng&owner_uid=0&tknv=v2&size=2048x2048 "Пример GET запроса")

Сообщения принимаются [консьюмером](https://github.com/d1mak3/DockerInfrastructure/tree/master/MessagesServer). В методе `public void GetQueryFromTheQueue()` класса `RabbitController` происходит парсинг команды, посланной продюссером, после чего выполняется операция, указанная в команде.

____

## Подключение к базе данных
Для подключения к бд я создал интерфейс `IDatabaseController`, который реализуется классом `MySqlDatabaseController` для подключения к MySQL. За сохранение сообщений отвечает метод `public bool SaveMessage(Models.Message messageToSave)`, а за получение истории сообщений — `public List<Models.Message> GetMessages()`. 

____

## Кэширование
Для кэширования (а точнее подключения к redis) я создал интерфейс `ICacheController`, который реализуется классом `RedisController` для подключения к redis. За поиск значения по ключу отвечает метод `public string GetValueByKey(string key)`, а за изменение/создания значения используется `public bool SetValue(string key, string value)`.

____