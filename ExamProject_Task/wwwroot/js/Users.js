$(document).ready(function () {
    // تحميل المستخدمين عند فتح الصفحة
    loadUsers();

    // إضافة مستخدم جديد
    $("#addUserForm").submit(function (event) {
        event.preventDefault(); // منع إعادة تحميل الصفحة

        var username = $("#username").val();
        var password = $("#password").val();

        $.ajax({
            url: "/Admin/AddUser",  // استبدلها بمسار الـ Controller
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ username: username, password: password }),
            success: function (response) {
                alert("User added successfully!");
                $("#addUserModal").modal("hide"); // إغلاق المودال بعد الإضافة
                $("#addUserForm")[0].reset(); // إعادة تعيين الفورم
                loadUsers(); // تحميل المستخدمين بعد الإضافة
            },
            error: function () {
                alert("Error adding user.");
            }
        });
    });

    // تحميل المستخدمين في القائمة المنسدلة
    function loadUsers() {
        $.ajax({
            url: "/Admin/GetUsers",
            type: "GET",
            success: function (data) {
                var userList = $("#userList");
                userList.empty();
                userList.append('<option value="">Select a User</option>');
                $.each(data, function (index, user) {
                    userList.append('<option value="' + user.id + '">' + user.username + '</option>');
                });
            },
            error: function () {
                alert("Error loading users.");
            }
        });
    }
});