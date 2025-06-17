---
### БД
Стандартная строка подключения 
```
server=localhost;user=root;database=officesupplies;password=root;
```

Поменять можно в `dbhelper.cs`

### ! Если меняли строку подключения то установщик надо пересобирать !

в visual studio проект `officesupplies` -> ПКМ -> Собрать

Установщик лежит в `officesupplies/Debug/`

---
### Стандартные пользователи 
Предусмотрено 2 роли

#### Администратор
login/password : `admin/admin`

#### Менеджер
login/password : `manager/manager`

---

### Вспомогательные классы в проекте
БОльшая часть обработок прописана в файлах с формами

для работы с бд : `dbhelper.cs`
для хэширования паролей : `Hasher.cs`
информация о пользователе : `UserInfo.cs`

---

#### Таймер бездействия 

Прописан в формах `MainAdminForm` и `MainManagerForm`
Для теста можно менять время таймера. Переменная с количеством секунд в таймере `inactivityLimit`

---

