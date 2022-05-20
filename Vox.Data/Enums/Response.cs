using System;

namespace Vox.Data.Enums;

public enum Response : byte
{
    SuccessfullyReceivedRoles,
    SuccessfullyRemovedRoles,
    SuccessfullyReceivedRole,
    SuccessfullyRemovedRole,
    SuccessfullyCreatedRolePicker,
    SelectRolesFromDropdown,
    FieldName,
    FieldOwner,
    FieldMembersCount,
    FieldRolesCount,
    FieldTextChannelsCount,
    FieldVoiceChannelsCount,
    FieldCreatedAt,
    FieldJoinedAt,
    FieldMention,
    FieldId,
    FieldColor,
    ViewGuild,
    ViewRole,
    ViewUser,
    ViewGuildRoles,
    ViewRoleMembers,
    Back,
    Forward,
    FooterPage,
    Bot,
    User,
    Role,
    Channel,
    BotTag,
    UserTag,
    GuildRoleInfo,
    CreateRolePicker,
    SettingsCreateChannels,
    SettingsCreateChannelDesc,
    SettingsCreateChannelFieldDesc,
    SettingsCreateChannelButtonCreate,
    SettingsCreateChannelButtonDelete,
    DeleteCreateChannelPlaceholder,
    DeleteCreateChannelOption,
    DeleteCreateChannelSelectedDesc,
    NewCreateChannelDesc,
    NewCreatedChannelCategoryName,
    NewCreatedChannelName,
    CreatePoll,
    SuccessfullyCreatedPoll,
    Poll,
    PollQuestion,
    PollDuration,
    SelectAnswersFromDropdown,
    PollAnswerDesc,
    SettingsCreateChannelsLimit,
    SettingsCreateChannelsLimitDesc,
    PollResults,
    PollMaxAnswers,
    Minutes,
    Answers,
    PollFooter,

    // Exceptions
    SomethingWentWrongTitle,
    SomethingWentWrongDesc,
    RoleNotFoundInGuild,
    WrongRolesCount,
    WrongAnswersCount,
    CreateChannelLimitation,
    MaxCharsLimitation,
    PollAnswersLimitation,
    PollAnswersAlready
}

public static class ResponseHelper
{
    private static string Localize(this Response response, Language language)
    {
        return response switch
        {
            Response.SuccessfullyReceivedRoles => language switch
            {
                Language.English => "You successfully received the roles: {0}.\n",
                Language.Russian => "Вы успешно получили роли: {0}.\n",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SuccessfullyRemovedRoles => language switch
            {
                Language.English => "You successfully removed the roles: {0}.",
                Language.Russian => "Вы успешно сняли роли: {0}.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SuccessfullyReceivedRole => language switch
            {
                Language.English => "You successfully received the role {0}.",
                Language.Russian => "Вы успешно получили роль {0}.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SuccessfullyRemovedRole => language switch
            {
                Language.English => "You successfully removed the role {0}.",
                Language.Russian => "Вы успешно сняли роль {0}.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.RoleNotFoundInGuild => language switch
            {
                Language.English => "Role with ID {0} not found in guild with ID {1}",
                Language.Russian => "Роль с ID {0} не найдена на сервере с ID {1}",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.WrongRolesCount => language switch
            {
                Language.English => "the number of roles must be greater than or equal to 1 and less than 26",
                Language.Russian => "количество ролей должно быть больше или равно 1 и меньше 26",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SuccessfullyCreatedRolePicker => language switch
            {
                Language.English => "{0}, role picker created successfully.",
                Language.Russian => "{0}, выбор ролей успешно создан.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SelectRolesFromDropdown => language switch
            {
                Language.English => "Select roles from the dropdown list",
                Language.Russian => "Выберите роли из выпадающего списка",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldName => language switch
            {
                Language.English => "{0} Name",
                Language.Russian => "{0} Название",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldOwner => language switch
            {
                Language.English => "{0} Owner",
                Language.Russian => "{0} Владелец",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldMembersCount => language switch
            {
                Language.English => "{0} Members count",
                Language.Russian => "{0} Количество пользователей",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldRolesCount => language switch
            {
                Language.English => "{0} Roles count",
                Language.Russian => "{0} Количество ролей",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldTextChannelsCount => language switch
            {
                Language.English => "{0} Text channels count",
                Language.Russian => "{0} Количество текстовых каналов",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldVoiceChannelsCount => language switch
            {
                Language.English => "{0} Voice channels count",
                Language.Russian => "{0} Количество голосовых каналов",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldCreatedAt => language switch
            {
                Language.English => "{0} Created at",
                Language.Russian => "{0} Дата создания",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.ViewGuildRoles => language switch
            {
                Language.English => "View guild roles",
                Language.Russian => "Посмотреть серверные роли",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldMention => language switch
            {
                Language.English => "{0} Mention",
                Language.Russian => "{0} Упоминание",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldId => language switch
            {
                Language.English => "{0} ID",
                Language.Russian => "{0} ID",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldColor => language switch
            {
                Language.English => "{0} Color",
                Language.Russian => "{0} Цвет",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.ViewRoleMembers => language switch
            {
                Language.English => "View role members",
                Language.Russian => "Посмотреть пользователей с ролью",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FieldJoinedAt => language switch
            {
                Language.English => "{0} Joined at",
                Language.Russian => "{0} Дата присоединения",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.Back => language switch
            {
                Language.English => "Back",
                Language.Russian => "Назад",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.Forward => language switch
            {
                Language.English => "Forward",
                Language.Russian => "Вперед",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.FooterPage => language switch
            {
                Language.English => "Page {0} of {1}.",
                Language.Russian => "Страница {0} из {1}.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.BotTag => language switch
            {
                Language.English => "{0} Bot",
                Language.Russian => "{0} Бот",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.UserTag => language switch
            {
                Language.English => "{0} User",
                Language.Russian => "{0} Пользователь",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.GuildRoleInfo => language switch
            {
                Language.English => "`{0}` {1}:\n{{ {2} color: {3}, {4} members count: {4} {5} {6}, {7} {8} {9} }}\n\n",
                Language.Russian => "`{0}` {1}:\n{{ {2} цвет: {3}, {4} количество: {4} {5} {6}, {7} {8} {9} }}\n\n",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.Bot => language switch
            {
                Language.English => "bot",
                Language.Russian => "бот",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.User => language switch
            {
                Language.English => "user",
                Language.Russian => "пользователь",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.ViewGuild => language switch
            {
                Language.English => "View guild",
                Language.Russian => "Просмотр сервера",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.ViewRole => language switch
            {
                Language.English => "View role",
                Language.Russian => "Просмотр роли",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.ViewUser => language switch
            {
                Language.English => "View user",
                Language.Russian => "Просмотр пользователя",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.Role => language switch
            {
                Language.English => "role",
                Language.Russian => "роль",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.Channel => language switch
            {
                Language.English => "channel",
                Language.Russian => "канал",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SomethingWentWrongTitle => language switch
            {
                Language.English => "Oops, looks like something went wrong...",
                Language.Russian => "Ой, кажется что-то пошло не так...",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SomethingWentWrongDesc => language switch
            {
                Language.English =>
                    "{0}, something unusual happened and I already reported it to the development team. I apologize for my dummy creators, they will definitely improve.",
                Language.Russian =>
                    "{0}, произошло что-то необычное и я уже сообщила об этом команде разработки. Приношу извинения за моих глупых создателей, они обязательно исправятся.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.CreateRolePicker => language switch
            {
                Language.English => "Create role picker",
                Language.Russian => "Создать выбор роли",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SettingsCreateChannels => language switch
            {
                Language.English => "Settings: Create channels",
                Language.Russian => "Настройки: Создаваемые каналы",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SettingsCreateChannelDesc => language switch
            {
                Language.English => ".",
                Language.Russian => ".",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SettingsCreateChannelFieldDesc => language switch
            {
                Language.English => "`{0}` Create channel {1} in category **{2}**",
                Language.Russian => "`{0}` Создаваемый канал {1} в категории **{2}**",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SettingsCreateChannelButtonCreate => language switch
            {
                Language.English => "Create new category for created channels",
                Language.Russian => "Создать новую категория для создаваемых каналов",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SettingsCreateChannelButtonDelete => language switch
            {
                Language.English => "Delete categories of created channels",
                Language.Russian => "Удалить категории создаваемых каналов",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.DeleteCreateChannelPlaceholder => language switch
            {
                Language.English => "Select the categories you want to delete",
                Language.Russian => "Выберите категории которые вы хотите удалить",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.DeleteCreateChannelOption => language switch
            {
                Language.English => "{0} in category {1}",
                Language.Russian => "{0} в категории {1}",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.DeleteCreateChannelSelectedDesc => language switch
            {
                Language.English => "{0}, selected categories successfully deleted.",
                Language.Russian => "{0}, выбранные категории успешно удалены.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.NewCreateChannelDesc => language switch
            {
                Language.English =>
                    "{0}, you successfully created new create channel {1}.\nYou can {2} rename category and channel as you like, bot works by {3} id of these channels.",
                Language.Russian =>
                    "{0}, вы успешно создали новый создаваемый канал {1}.\nВы можете {2} переименовать категорию и канал как вам хочется, бот работает по {3} айди этих каналов.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.CreateChannelLimitation => language switch
            {
                Language.English => "you cannot create more than 3 categories of created channels",
                Language.Russian => "вы не можете создать более 3 категорий создаваемых каналов",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.NewCreatedChannelCategoryName => language switch
            {
                Language.English => "Created channels",
                Language.Russian => "Созданные каналы",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.NewCreatedChannelName => language switch
            {
                Language.English => "Create channel",
                Language.Russian => "Создать канал",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.CreatePoll => language switch
            {
                Language.English => "Create poll",
                Language.Russian => "Создать опрос",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SuccessfullyCreatedPoll => language switch
            {
                Language.English => "{0}, poll successfully created.",
                Language.Russian => "{0} опрос успешно создан.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.Poll => language switch
            {
                Language.English => "Poll",
                Language.Russian => "Опрос",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.PollQuestion => language switch
            {
                Language.English => "{0} Question",
                Language.Russian => "{0} Вопрос",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.PollDuration => language switch
            {
                Language.English => "{0} Duration",
                Language.Russian => "{0} Длительность",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SelectAnswersFromDropdown => language switch
            {
                Language.English => "Select an answer from the dropdown list",
                Language.Russian => "Выберите ответ из выпадающего списка ",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.WrongAnswersCount => language switch
            {
                Language.English => "the number of answers must be greater than 1 and less than 26",
                Language.Russian => "количество ответов должно быть больше 1 и меньше чем 26",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.MaxCharsLimitation => language switch
            {
                Language.English => "maximum number of characters - {0}",
                Language.Russian => "максимальное количество символов - {0}",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.PollAnswerDesc => language switch
            {
                Language.English => "{0}, your answer successfully added: {1}.",
                Language.Russian => "{0}, ваш ответ успешно принят: {1}.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.PollAnswersLimitation => language switch
            {
                Language.English => "maximum number of answers - {0}",
                Language.Russian => "максимальное количество ответов на этом опросе - {0}",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.PollAnswersAlready => language switch
            {
                Language.English => "You have already answered this poll",
                Language.Russian => "вы уже выбирали этот ответ на этом опросе",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SettingsCreateChannelsLimit => language switch
            {
                Language.English => "Settings: Create channels user limit",
                Language.Russian => "Настройки: Ограничение пользователей в созданном канале",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.SettingsCreateChannelsLimitDesc => language switch
            {
                Language.English => "{0}, you successfully set the limit of users in the created channels to {1}.",
                Language.Russian => "{0}, вы успешно установили ограничение пользователей в созданном канале до {1}.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.PollResults => language switch
            {
                Language.English => "{0} Poll results",
                Language.Russian => "{0} Результаты опроса",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.PollMaxAnswers => language switch
            {
                Language.English => "{0} Maximum number of answers",
                Language.Russian => "{0} Максимальное количество ответов",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.Minutes => language switch
            {
                Language.English => "minute",
                Language.Russian => "минута",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.Answers => language switch
            {
                Language.English => "answer",
                Language.Russian => "ответ",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            Response.PollFooter => language switch
            {
                Language.English => "Your response will be completely anonymous.",
                Language.Russian => "Ваш ответ будет полностью анонимным.",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(response), response, null)
        };
    }

    public static string Parse(this Response response, string preferredLocale)
    {
        return response.Localize(preferredLocale.GetLanguageFromPreferredLocale());
    }

    public static string Parse(this Response response, string preferredLocale, params object[] replacements)
    {
        try
        {
            return string.Format(Parse(response, preferredLocale), replacements);
        }
        catch (FormatException)
        {
            return "`An error occurred while displaying the response. Please show this on support server.`";
        }
    }
}