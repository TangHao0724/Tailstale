$(document).ready(function () {
    var infoaJson; // userID,Name
    var selectUserID; //ID
    var userDetailJson;
    var imgDetail;
    var imgtypeID;//typeID

    const UMapi = axios.create({
        baseURL: '/api/UserMangerApi/',
        timeout: 1000,
    })

    const Imgapi = axios.create({
        baseURL: '/api/ImgApi/',
        timeout: 1000,
    })
    //function

    function isJson(str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    }

    //特殊
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
                    reflashImgDetail();


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
            reflashInfoDetail();
            UserInfoBind();


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
        $("#nav-info #UserDeleteModal #selectID").text(`${selectUserID}`);//為什麼沒有
        //刪除帳號
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
                    alert("已經修改 編號：" + JSON.stringify(response.data.userId));
                    $('#UserUpdateModal').modal('hide');
                    reflashInfoDetail();

                }).catch(function (error) {
                    console.error('API 請求失敗', error);
                    alert('你有問題');
                    // 处理错误信息
                })
        })

    }//刷新時綁定Info物件,

    var reflashPetpage = function () {
        UMapi.get('/userPetPage', {
            params: {
                ID: selectUserID
            }
        }).then(response => {
            // 處理成功的回應
            console.log(response.data);
            $("#nav-petInfo").html(response.data);
            reflashInfoDetail();
            UserInfoBind();


        }).catch(error => {
            // 處理錯誤
            console.error('發生錯誤：', error)
        });
    }//刷新Pet頁面
    var reflashPetDetail = function () {

    }//生成Pet內容
    var PetBind = function () {

    }//生成綁定內容
    var reflashImgPage = function () {
        UMapi.get('/userImgPage', {
            params: {
                ID: selectUserID
            }
        }).then(response => {
            // 處理成功的回應
            console.log(response.data);;
            $("#nav-img").html(response.data);
            $('#imgTypeTable').DataTable({
                columns: [
                    {
                        "data": "id",
                        width: '20%',
                    },
                    {
                        "data": "typename", 
                        width: "50%",
                        render: function (data, type, row) {
                            return '<a class="my_a link-dark text-decoration-none" data-id="' + row.id + '" style="cursor: pointer;">' + data + '</a>'
                        }

                    },
                    {
                        "data": null,
                        "defaultContent": '<button class="btn btn-primary btn-sm">選擇</button>',
                        "orderable": false,
                        width: "30%",
                        
                    },

                ],
                retrieve:true,
                paging: false,
                scrollY: '50vh',
                scrollCollapse: true,
                deferRender: true,

                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.13.1/i18n/zh-HANT.json'
                }
            });


            reflashImgDetail();
            ImgBind();


        }).catch(error => {
            // 處理錯誤     
            console.error('發生錯誤：', error)
            alert(error.stringify);
        });

    }//刷新圖片內容

    var reflashImgDetail = function () {
        Imgapi.get("/userImgType", {
            params: {
                ID: selectUserID
            }
        }).then(response => {
            console.log(response.data)
            $('#imgTypeTable').DataTable().clear().rows.add(response.data).draw();
            $('#imgTypeTable tbody').on('click', 'button', function () {
                data = $('#imgTypeTable').DataTable().row($(this).parents('tr')).data().id;
                imgtypeID = data;
                //進入下一層img_type
            })



        }).catch(error => {
            console.log(error)
        })

    }//刷新圖片內容

    
    var ImgBind = function () {
        //點擊相簿名稱的反應
        $('#imgTypeTable').on('click', '.my_a', function () {
            alert('您點擊了按鈕！');
        });
        //上傳圖片並相片種類
        $("#InsertImg").submit(function (event) {
            event.preventDefault();

            var ImgData = new FormData(this);


            ImgData.append("User_id", selectUserID);

            Imgapi.post("UploadsingleImg", ImgData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
                .then(function (response) {
                    console.log('API 返回:', response.data);
                    alert(JSON.stringify(response.data))
                    $('#InsertImgModal').modal('hide');
                    reflashImgDetail();
                })
                .catch(function (error) {
                    console.error('API 請求失敗', error);
                    alert(error.stringify);
                    // 处理错误信息
                })

        })
        //插入相簿
        $("#InsertImgType").submit(function (event) {
            event.preventDefault();

            var imgData = {
                User_id: selectUserID,
                type_name: $('#type_name').val(),
            };
            Imgapi.post("InsertImgType", imgData)
                .then(function (response) {
                    console.log('API 返回:', response.data);
                    alert(JSON.stringify(response.data))
                    $('#InsertImgTypeModal').modal('hide');
                    reflashImgDetail();
                })
                .catch(function (error) {
                    console.error('API 請求失敗', error);
                    alert(error.stringify);
                    // 处理错误信息
                })

        })

        //刪除相片
        $(" #Deleteimg").on("click", function () {

            Imgapi.post("DeleteImgType",
                { ID: imgtypeID },
                { headers: { 'Content-Type': 'application/json' } }
            ).then(response => {
                console.log(response.data)
                alert(JSON.stringify(response.data))
                reflashImgDetail();
            }).catch(function (error) {
                console.error('請求失敗', error);
                alert(error.stringify);
                // 处理错误信息
            })
            $(document).on('click', '.row-link', function (e) {
                e.preventDefault();
                var rowId = $(this).data('id');
                console.log("連結已被點擊，該行的 ID 是：" + rowId);
            });

        })



    }//綁定到Imgpage物件


    //function

    //表格
    $('#table').DataTable({
        columns: [
            { "data": "id",  },
            {
                data: "name",
            },
            {
                "data": null,
                "defaultContent": '<button class="btn btn-primary btn-sm">選擇</button>',
                "orderable": false,
               
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
        $("nav-info").empty();
        reflashInfoPage();
    })
    $("#img-tab").on("click", function () {
        $("#nav-info").empty();
        reflashImgPage();

        //打開IMGtype分頁
    })

    $("#petInfo-tab").on("click", function () {
        
        reflashPetpage();

        
    })//打開Pet分頁

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
