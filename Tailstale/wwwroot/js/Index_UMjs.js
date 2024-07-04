$(document).ready(function () {
    var infoaJson; // userID,Name
    var selectUserID; //ID
    var userDetailJson;

    const UMapi = axios.create({
        baseURL: '/api/UserMangerApi/',
        timeout: 1000,
    })
    const Imgapi = axios.create({
        baseURL: '/api/ImgApi/',
        timeout: 1000,
    })
    //function
    
    var reflashDataTable = function () {
        
        UMapi.get('/userInfo')
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
        UMapi.get('/userInfoPage', {
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
        UMapi.get('/userInfoDetail', {
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

                $("#updateName").val(userDetailJson.name)
                $("#updateEmail").val(userDetailJson.email)
                $("#updateAddress").val(userDetailJson.address)
                $("#updatePhone").val(userDetailJson.phone)
                $("#updatePassword").val(userDetailJson.phone)
                $("#updateStatus").val(userDetailJson.status)
            })
            .catch(error => {
                // 處理錯誤
                console.error('發生錯誤：', error)
            });
    }//更新Info讀取的資料
    var UserInfoBind = function () {
        
        if ($('#deleteTBTN').length > 0) {
            console.log('The element with the specified ID has been inserted.');

        } else {
            console.log('The element with the specified ID has not been inserted.');
            //刪除帳號
            $("#nav-info #UserDeleteModal #selectID").text(`${selectUserID}`);//為什麼沒有
            $(" #nav-info #deleteTBTN").on("click", function () {
                UMapi.post('/DeleteUser',
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
                UMapi.post('/UpdateUser', UpdateData)
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

        }


    }//刷新時綁定Info物件,



    var reflashImgPage = function () {
        UMapi.get('/userImgPage', {
            params: {
                ID: selectUserID
            }
        }).then(response => {
            // 處理成功的回應
            console.log(response.data);;
            $("#nav-img").html(response.data);
            ImgBind();
            $("deleteBTN").on("click", function () {
                alert("sad")
            })



        }).catch(error => {
            // 處理錯誤
            console.error('發生錯誤：', error)
        });

    }
    var ImgBind = function () {
        //上傳圖片
        $("InsertImg").submit(function (event) {
            var ImgData = {
                User_id: selectUserID,
                img: $("insertImg").val(),
                type_name: $('#insertImgtype').val(),
                Imgname: $('#insertImgName').val(),
            };
            Imgapi.post("UploadsingleImg", ImgData)
                .then(function (response) {
                    console.log('API 返回:', response.data);

                    $('#InsertImgModal').modal('hide');
                })
                .catch(function (error) {
                    console.error('API 請求失敗', error);
                    alert('建立失敗');
                    // 处理错误信息
                })

        })


    }//綁定到Imgpage物件


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
    //點即時打開頁面
    $("#info-tab").on("click", function () {

        reflashInfoPage();
    })
    $("#img-tab").on("click", function () {
        $("#nav-info").html('');
               UMapi.get('/userImgPage', {
            params: {
                ID: selectUserID
            }
        }).then(response => {
            // 處理成功的回應
            console.log(response.data);;
            $("#nav-img").html(response.data);
            ImgBind();
            $("deleteBTN").on("click", function () {
                alert("sad")
            })



        }).catch(error => {
            // 處理錯誤
            console.error('發生錯誤：', error)
        });

    })

    //建立新帳號API
    $('#insertUser').submit(function (event) {
        event.preventDefault();

        var UserformData = {
            
            name: $('#inputName').val(),
            email: $('#inputEmail').val(),
            password: $('#inputPassword').val(),
            address: $('#inputAddress').val(),
            phone: $('#inputPhone').val()

        };
        UMapi.post('/PostUser', UserformData)
            .then(function (response) {
                console.log('API 返回:', response.data);

                $('#UserInsertModal').modal('hide');
                reflashDataTable();
            })
            .catch(function (error) {
                console.error('API 請求失敗', error);
                alert('建立失敗');
                // 处理错误信息
            })

    });

    
});
