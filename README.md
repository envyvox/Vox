# Vox

## Как добавить и посмотреть работу бота?
1. [Добавляем бота на необходимый сервер.](https://discord.com/oauth2/authorize?client_id=955433000613584926&scope=applications.commands%20bot&permissions=0)
2. По-умолчанию бот просит права администратора, их можно не выдавать и настроить его права самостоятельно, однако в таком случае какой-то из функционала может не работать из-за нехватки прав доступа.
3. После добавления бота дискорд прогрузит на ваш сервер его слэш-команды (это может занять некоторое время, по словам дискорда).
4. Вводим `/` и должны увидеть команды бота, которые можно использовать.

## Как мне захостить бота самостоятельно?
Я, конечно же, не рекомендую этого делать по множеству причин, однако если прям нужно, то:
1. Скачиваем архив / клоним гит, как угодно.
2. Для запуска боту необходимы некоторые `environmentVariables`, а точнее:
- `CUSTOMCONNSTR_main` - соединение с базой, тут все понятно.
- `Vox_DiscordOptions__Token` - токен вашего бота дискорда.
- `Vox_DiscordOptions__TestingGuildId` - необходимо указать ID сервера дискорда, на который будут загружены слэш-команды при локальном запуске (дискорд очень не рекомендует загружать слэш-команды глобально при локальном запуске, оно и понятно, кидать им реквесты на релоад всех команд при каждом перезапуске бота локально это ад)
- `Vox_DiscordOptions__IconGuildsId` - и вот она часть по которой самостоятельный хостинг это плохо. Бот использует иконки в своих сообщениях, эти иконки лежат на приватных серверах которые указываются тут через `;`. Сервера с иконками я вам не дам, я жадный.
- `Vox_CronTimezoneId` - таймзона по которой стоит работать боту. Ее хенгфайр кушает, правда уже и не помню обязательна ли она там.
3. Запускаем.
4. Если что-то не получилось, вполне может быть что я вообще забыл что-то упомянуть связанное с самостоятельным запуском этой штуки. Напишите мне куда угодно и я подскажу. Но лучше просто добавьте бота по ссылке в самом начале.
