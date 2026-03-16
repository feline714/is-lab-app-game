var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 1. Диагностические эндпоинты (Требование Задания 3)
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

app.MapGet("/version", (IConfiguration config) => 
    new { appName = config["App:Name"], version = config["App:Version"] });

// 2. Эндпоинт для будущей БД (Требование Задания 5)
app.MapGet("/db/ping", (IConfiguration config) => {
    var connectionString = config.GetConnectionString("Mssql");
    return Results.Problem("БД пока недоступна. SQL Server еще не развернут.");
});


// 3. НАША МИНИ-ИГРА: Камень, ножницы, бумага (Вместо Задания 4)
// Массив возможных вариантов
string[] options = { "камень", "ножницы", "бумага" };

app.MapGet("/api/play", (string choice) => {
    // Переводим выбор пользователя в нижний регистр, чтобы избежать ошибок (например, Камень -> камень)
    choice = choice.ToLower();

    // Проверка: если ввели какую-то ерунду
    if (!options.Contains(choice))
    {
        return Results.BadRequest(new { error = "Неверный ввод! Выберите: камень, ножницы или бумага." });
    }

    // Сервер делает случайный выбор
    string serverChoice = options[new Random().Next(options.Length)];
    string resultMessage;

    // Логика игры
    if (choice == serverChoice)
    {
        resultMessage = "Ничья!";
    }
    else if ((choice == "камень" && serverChoice == "ножницы") ||
             (choice == "ножницы" && serverChoice == "бумага") ||
             (choice == "бумага" && serverChoice == "камень"))
    {
        resultMessage = "Вы победили! 🏆";
    }
    else
    {
        resultMessage = "Сервер победил! 🤖";
    }

    // Возвращаем результат в формате JSON
    return Results.Ok(new 
    { 
        playerChoice = choice, 
        serverChoice = serverChoice, 
        result = resultMessage 
    });
});

// Запуск приложения
app.Run();