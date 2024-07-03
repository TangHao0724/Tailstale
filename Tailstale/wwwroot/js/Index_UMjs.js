$(document).ready(function () {
    var infoaJson; // userID,Name
    var selectUserID; //ID
    var userDetailJson;

    //function
    
    var reflashDataTable = function () {
        
        axios.get('/api/UserMangerApi/userInfo')
            .then(response => {
                // 處理成功的回應
                console.log('API 回應資料：', response.data);
                infoa = response.data;

                $('#table').DataTable().clear().rows.add(infoa).draw();

                $('#table tbody').on('click', 'button', function () {
                    data = $('#table').DataTable().row($(this).parents('tr')).data().id;

                    selectUserID = data;

                    $("#selectID").text(selectUserID);
                    if ($(".nav-link").hasClass("disabled")) {
                        $(".nav-link").removeClass("disabled")
                    }
                    $("#nav-info").show();
                    reflashInfoDetail();
                    UserInfoBind();
                    //如果目前號碼等於selectUserID Disabled選擇紐
                });
            })
            .catch(error => {
                // 處理錯誤
                console.error('發生錯誤：', error)
            });
    }//更新資料後刷新旁邊的表
    var reflashInfoPage = function () {
        axios.get('/api/UserMangerApi/userInfoPage', {
            params: {
                ID: selectUserID
            }
        }).then(response => {
            // 處理成功的回應
            console.log(response.data);
            $("#nav-info").html(response.data);
            UserInfoBind();
            reflashInfoDetail();
            

        }).catch(error => {
            // 處理錯誤
            console.error('發生錯誤：', error)
        });
            
           
    }// 刷新Info頁面
    var reflashInfoDetail = function () {
        axios.get('/api/UserMangerApi/userInfoDetail', {
            params: {
                ID: selectUserID
            }
        })
            .then(response => {
                // 處理成功的回應
                console.log(response.data);
                userDetailJson = response.data;
                $('#selectedID').text(`${JSON.stringify(response.data.id)}`);
                $('#selectedName').text(`${response.data.name}`);
                $("#selectedPassword").text(`${response.data.password}`);
                $('#selectedPhone').text(`${response.data.phone}`);
                $('#selectedEmail').text(`${response.data.email}`);
                $('#selectedAddress').text(`${response.data.address}`);
                $('#selectedCreated').text(`${response.data.created_at}`);
                let Statusarr = ['無狀態', '尚未驗證', '已驗證', '停權']
                $('#selectedStatus').text(`${Statusarr[JSON.stringify(response.data.status)]}`);

                insertValue();
            })
            .catch(error => {
                // 處理錯誤
                console.error('發生錯誤：', error)
            });
    }//更新Info讀取的資料
    var UserInfoBind = function () {
        
        
        //刪除帳號
        $("#nav-info #UserDeleteModal #seletID").text(selectUserID);
        $(" #nav-info #deleteTBTN").on("click", function () {
                axios.post('/api/UserMangerApi/DeleteUser',
                    { ID: selectUserID },
                    { headers: { 'Content-Type': 'application/json' } }
                )
                    .then(function (response) {

                        console.log('API 返回:', response.data);
                        alert(response.data);
                        $('#UserDeleteModal').modal('hide');
                        reflashDataTable();
                    })
                    .catch(function (error) {
                        console.error('API 請求失敗', error);
                    });
            });
        //更新USER
        $("#nav-info #UserUpdateModal #UpdateUser").submit(function (event) {
            event.preventDefault();

            var UpdateData = {
                ID: selectUserID,
                name: $('#updateName').val(),
                email: $('#updateEmail').val(),
                password: $('#updatePassword').val(),
                address: $('#updateAddress').val(),
                phone: $('#updatePhone').val(),
                status: $('#updateStatus').val(),

            };
            axios.post('/api/UserMangerApi/UpdateUser', UpdateData)
                .then(function (response) {
                    console.log('API 返回:', response.data);
                    alert(JSON.stringify(response.data.userId));
                    $('#UserUpdateModal').modal('hide');
                    reflashInfoDetail();

                }).catch(function (error) {
                    console.error('API 請求失敗', error);
                    alert('你有問題');
                    // 处理错误信息
                })
        })
        

    }//綁定deleteUser,

    var insertValue = function () {
        $("#updateName").val(userDetailJson.name)
        $("#updateEmail").val(userDetailJson.email)
        $("#updateAddress").val(userDetailJson.address)
        $("#updatePhone").val(userDetailJson.phone)
        $("#updatePassword").val(userDetailJson.phone)
        $("#updateStatus").val(userDetailJson.status)
    }

    //function

    //表格
    $('#table').DataTable({
        columns: [
            { "data": "id", "width": "20%" },
            { "data": "name", "width": "20%" },
            {
                "data": null,
                "defaultContent": '<button class="btn btn-primary btn-sm">選擇</button>',
                "orderable": false,
                "width": "20%"
            }
        ],
        paging: false,
        scrollY: '50vh',
        scrollCollapse: true,
        deferRender: true,

        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.1/i18n/zh-HANT.json'
        }
    });

    //初始加載
    //切換至info頁面
    reflashDataTable();
    $("#info-tab").on("click", function () {

        reflashInfoPage();
    })
    $("#DUBtn").on("click", function () {
        alert(selectUserID);
    })

    //建立新帳號
    $('#insertUser').submit(function (event) {
        event.preventDefault();

        var UserformData = {
            
            name: $('#inputName').val(),
            email: $('#inputEmail').val(),
            password: $('#inputPassword').val(),
            address: $('#inputAddress').val(),
            phone: $('#inputPhone').val()

        };

        axios.post('/api/UserMangerApi/PostUser', UserformData)
            .then(function (response) {
                console.log('API 返回:', response.data);
                alert('建立成功，用户ID: ' + JSON.stringify(response.data.userId));
                $('#UserUpdateModal').modal('hide');
                reflashDataTable();
            })
            .catch(function (error) {
                console.error('API 請求失敗', error);
                alert('建立失敗');
                // 处理错误信息
            })


    });
   
    
});
