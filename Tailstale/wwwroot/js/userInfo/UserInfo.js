﻿/*
1.建立側欄切煥功能
2. 顯示使用者資訊，並且使用model去修改內容。
3. 新增寵物的方法
4. 歷史貼文
*/

const app = Vue.createApp({
    data() {
        return {
            currentTab: '主頁',
            imgurlLu: "",
            tabs: ['主頁', '使用者資訊', '寵物資訊', '歷史訂單','相片集'],
            useridm:""
        }
    },
    computed: {
        currentTabComponent: function () {
            return `tab-${this.currentTab}`;
        }
    },
    methods: {
        logout() {
            axios.delete('api/LNRApi/Logout')
                .then(response => {
                    if (response.status === 200) {
                        window.location.href = '/User/Index';
                    } else {
                        console.error('Logout failed');
                    }
                })
                .catch(error => console.error('Error:', error));
        },
    },
    mounted() {
        this.useridm = $("#user-id").data('user-id');

    }
});


app.component('tab-主頁', {
    template: `#tab-Main`,
    data() {
        return {
            newpost: [],
            newpets: [],
            newresp: [],
            mainart:[],
            neworder: [],
            ewallresp:[],
            artcount: 0,
            newPictures: null,
            postL: 0,
            respL: 0,
            petsL: 0,
            orderL: 0,
            name: '',
            selectedArt: null,
            parentArt: [],
            pet_types: [],
            order:null
        }
    },
    created() {
        this.getnewPictures();
        this.getnewpost();
        this.getnewpets();
        this.getnewresp();
        this.getneworder();
        this.getartcount();
        this.fetchPetTypes();
        
    },
    props: {
        userid: Number,
    },
    methods: {
        imgurl(uType, imgurl) {
            if (imgurl === 'no_head.png') {
                return `imgs/keeper_img/${imgurl}`;
            }
            switch (uType) {
                case 0:
                    return `imgs/keeper_img/${imgurl}`;
                    break;
                case 1:
                    return `images/business/${imgurl}`;
                    break;
                case 2:
                    return '/Salon_img/'+`${imgurl}`;
                    break;
                case 3:
                    return 'lib/HospitalImages/'+`${imgurl}`;
                    break;
            }
        },
        async getnewpost() { 
            this.newpost = [];
            try {
                const response = await axios.get(`api/social/GetArticle`, {
                    params: {
                        id:this.userid,
                    }
                })
                var a = response.data;
                a.splice(3)
                this.newpost = a;
                this.name = this.newpost[0].kName;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        async getnewpets() {
            try {
                const response = await axios.get(`api/UserInfoApi/GetPet`, {
                    params: {
                        id: this.userid,
                    },
                });
                this.newpets = response.data;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        async getnewresp() {
            try {
                const response = await axios.get(`api/UserInfoApi/getArticleReplies`, {
                    params: {
                        id: this.userid,
                    },
                });
                this.newresp = response.data;
                var allrespde = [];
                for (const item of this.newresp) {
                    const response = await axios.get(`api/social/GetArticle`, {
                        params: {
                            artID: item.id,
                        }
                    })
                    allrespde.push(response.data);
                    this.newallresp = allrespde.flat();
                    console.log(this.newallresp);
                }
                var templist = [];
                for (const item of this.newallresp) {
                    const response = await axios.get(`api/social/GetArticle`, {
                        params: {
                            artID: item.parent_ID,
                        }
                    })
                    templist.push(response.data);
                    this.mainart = templist.flat();
                    console.log(this.newallresp);
                }
                this.respL = this.newresp.length;
            }catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        async getneworder() {
            try {
                const response = await axios.get(`api/UserInfoApi/UserOrder/${this.userid}`);
                this.neworder = response.data;
                this.orderL = this.neworder.length - 1;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }

        },
        async getartcount() {
            try {
                const response = await axios.get(`api/UserInfoApi/getcount`, {
                    params: {
                        id: this.userid,
                    },
                });
                this.artcount = response.data;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        async getnewPictures() {
            try {
                const response = await axios.get(`api/UserInfoApi/newPictures`, {
                    params: {
                        id: this.userid,
                    },
                });
                this.newPictures = response.data;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        async openPost(input) {

            this.selectedArt = null;
            this.parentArt = null;
            this.selectedArt = this.mainart.find(x => x.id === input.parent_ID);
            try {
                const response = await axios.get(`api/social/GetArticle`, {
                    params: {
                        parentid: this.selectedArt.id,
                    }
                })
                this.parentArt = response.data;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
            $("#articleModal").modal("show");
        },

        getPetType(pet) {
            const pets = this.pet_types.find(p => p.id === pet.pet_type_ID);
            if (pets) {
                return `${pets.species}, ${pets.breed}`;
            } else {
                return "找不到該ID的寵物種類";
            }
        },
        async fetchPetTypes(userid) {
            try {
                const response = await axios.get('api/UserInfoApi/GetPetTypes');
                this.pet_types = response.data;
            } catch (error) {
                console.error('Error fetching pet types:', error);
            }
        },
        bindimgurl(url, uType) {
            if (uType !== 0) {
                return `imgs/business_img/${url}`;

            } else {
                return `imgs/keeper_img/${url}`;
            }
        },
    }
});

app.component('tab-使用者資訊', {
    template: `#tab-UInfo`,
    data() {
        return {
            user: null,
            imgurl: "",
            fileData: null
        }
        
    },
    created() {
        this.fetchUserInfo(this.userid); // 替換成你想要查詢的用戶 ID
        this.GetUserimg(this.userid);
    },
    props: {
        userid: Number,
    },
    methods: {
        async fetchUserInfo(userid) {
            try {
                const response = await axios.get('api/UserInfoApi/userInfoDetail', {
                    params: {
                        ID: userid,
                    },
                });
                this.user = response.data;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        async GetUserimg(userid) {
            try {
                const formData = {
                    'UserID': this.userid,
                    'type_name': this.userid + '_head',
                    "img_name": "head",
                };
                
                const response = await axios.post('api/KImg/GetSingleImg', formData);
                console.log(JSON.stringify(response.data));
                this.imgurl = `/imgs/keeper_img/${response.data.img_url}` ;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        //上傳圖片
        async postUserimgInfo(userid) {
            try {
                const formData = new FormData(this.$refs.form);
                formData.append('UserID', this.userid);
                formData.append("type_name", this.userid + '_head');
                formData.append("img_name", "head")
                
                const response = await axios.post('api/KImg/UploadsingleImg', formData);
                this.GetUserimg(this.userid);
                this.$emit('data-updated');
                $("#UserUpdateImgModal").modal("hide");
                console.log(JSON.stringify(response.data));
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        //更新會員資料x
        async postuserinfo(userid) {
            try {
                const formData = new FormData(this.$refs.userinfoform);
                formData.append('ID', this.userid);

                const response = await axios.put('api/UserInfoApi/updateKeeper', formData, {headers: {
                    'Content-Type': 'application/json'
                }
                });
                this.fetchUserInfo(this.userid);
                $("#UserUpdateModal").modal("hide");
                console.log(JSON.stringify(response.data));
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        handleFileChange(event) {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.fileData = e.target.result;
                };
                reader.readAsDataURL(file);
            }
        }
    },
});

app.component('tab-寵物資訊', {
    template: `#tab-PInfo`,
    data() {
        return {
            selectedPet: null,
            pet_types: [],
            pets: [],
            fileData: null,
            selectedname: "",
        }
    },
    created() {
        this.fetchPetTypes();
        this.fetchPet(this.userid);
    },
    computed: {
    },
    props: {
        userid: Number,
    },
    methods: {
        async fetchPetTypes(userid) {
            try {
                const response = await axios.get('api/UserInfoApi/GetPetTypes');
                this.pet_types = response.data;
            } catch (error) {
                console.error('Error fetching pet types:', error);
            }
        },
        async submitForm(userid) {
            const formData = new FormData(this.$refs.form);
            const data = {};
            formData.forEach((value, key) => {
                data[key] = value;
            });
            // 確保數字類型正確
            data.age = parseInt(data.age, 10);
            data.pet_type_ID = parseInt(data.pet_type_ID, 10);
            data.gender = data.gender === '1';

            // 添加 keeper_ID（如果需要的話）
            data.keeper_ID = this.userid;
            axios.post('api/UserInfoApi/PostPetInfo', data, {
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(response => {
                console.log(response.data);
                this.fetchPet(this.userid);
            })
            .catch(error => {
                console.error('Error:', error);
            });
        },
        async submitPDUForm() {
            try {
                const formData = new FormData(this.$refs.PDUform);
                const data = {};
                formData.forEach((value, key) => {
                    data[key] = value;
                });
                // 確保數字類型正確
                data.ID = this.selectedPet.id;
                data.age = parseInt(data.age, 10);
                data.pet_type_ID = parseInt(data.pet_type_ID, 10);
                data.gender = data.gender === '1';
                data.neutered = data.neutered === '1';
                data.pet_weight = parseFloat(data.pet_weight);

                console.log(JSON.stringify(data));
                const response = await axios.put('api/UserInfoApi/updatePetInfo', data, {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                this.fetchPet(this.userid);
                $("#PDUpdateModal").modal("hide");
                console.log(JSON.stringify(response.data));
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        async fetchPet(userid) {
            try {
                const response = await axios.get('api/UserInfoApi/GetPet', {
                    params: {
                        ID: userid,
                    }
                });
                this.pets = response.data;
            } catch (error) {
                console.error('Error fetching pet types:', error);
            }
        },
        getGender(pet) {
            let gender = pet?.gender;
            if (gender === null) {
                return "尚未添加資訊";
            }
            return gender ? '母' : '公';
        },
        getPetType(pet) {
            const pets = this.pet_types.find(p => p.id === pet.pet_type_ID);
            if (pets) {
                return `${pets.species}, ${pets.breed}`;
            } else {
                return "找不到該ID的寵物種類";
            }
        },
        getSPetW(pet) {
            let weight = pet?.pet_weight;
            if (weight === null ) {
                return "尚未添加資訊";
            }
            return weight;
        },
        getSPetN(pet) {
            let neutered = pet?.neutered;
            if (neutered) {
                return "尚未添加資訊";
            }
            return neutered ? "已結紮" : "尚未結紮";
        },
        getPetimg(pet) {
            return `/imgs/keeper_img/${pet.imgurl}` ;
        },
        updateNeutered(value) {
            this.selectedPet.neutered = value;
        },
        openModal(pet) {
            this.selectedPet = null;
            this.selectedPet = pet;
        },
        //新增寵物圖片
        async postPImg() {
            try {
                const formData = new FormData(this.$refs.PIform);
                formData.append('UserID', this.userid);
                formData.append("type_name", 'pet_'+this.selectedname + '_head');
                formData.append("img_name", "head")

                const response = await axios.post('api/KImg/UploadsingleImg', formData);
                this.fetchPet(this.userid),
                $("#PImgModal").modal("hide");
                console.log(JSON.stringify(response.data));
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        //更新詳細資訊

        openDetailModal() {
            $("#PDUpdateModal").modal("show");
            $("#PDetailModal").modal("hide");

        },
        openChangeimgModel(pet) {
            this.selectedname = null;
            this.selectedname = pet.id + "_" + pet.name;
            console.log(this.selectedname);
            $("#PImgModal").modal("show");
            $("#PDetailModal").modal("hide");
            
        },
        handleFileChange(event) {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.fileData = e.target.result;
                };
                reader.readAsDataURL(file);
            }
        }
    },
    mounted() { 
        // 初始化 tempNeutered 的值
        if (this.selectedPet) {
            this.selectedPet.neutered = this.selectedPet.neutered ?? false;
        }
    }
    
});

app.component('tab-歷史訂單', {
    template: `#tab-HisOrder`,
    data() {
        return {
            userOrder: [],
            selectedtype: "",
            selectedorderID: null,
            
        }
    },
    created() {
        this.getAllOrder();
        $(document).ready(function () {
            const table = $('#orderTable').DataTable({
                language: {
                    "sProcessing": "處理中...",
                    "sLengthMenu": "顯示 _MENU_ 項結果",
                    "sZeroRecords": "沒有匹配結果",
                    "sInfo": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
                    "sInfoEmpty": "顯示第 0 至 0 項結果，共 0 項",
                    "sInfoFiltered": "(由 _MAX_ 項結果過濾)",
                    "sSearch": "搜尋：",
                    "sEmptyTable": "表中數據為空",
                    "sLoadingRecords": "載入中...",
                    "oPaginate": {
                        "sFirst": "首頁",
                        "sPrevious": "上一頁",
                        "sNext": "下一頁",
                        "sLast": "末頁"
                    },
                    "oAria": {
                        "sSortAscending": "以升序排列此列",
                        "sSortDescending": "以降序排列此列"
                    }
                },
                "order": [[0, "desc"]],
                columns: [
                    { title: "編號", data: "orderID" },
                    { title: "寵物名稱", data: "orderPet" },
                    { title: "服務種類", data: "orderType" },
                    { title: "店家名稱", data: "businessName" },
                    { title: "服務名稱", data: "serviceName" },
                    { title: "預約時間", data: "orderDate" },
                    { title: "預約狀態", data: "orderStatus" },
                    {
                        title: "查看詳情",
                        data: null,
                        defaultContent: '<button class="btn btn-outline-primary btn-action" @@click="switchUrl">詳情</button>',
                        orderable: false
                    }
                ],
                createdRow: function (row, data, dataIndex) {
                    $('td:eq(7)', row).html('<button class="btn btn-outline-primary btn-action" @@click="switchUrl">詳情</button>');
                }

            }); 
            $('#orderTable tbody').on('click', 'button.btn-action', (event) => {
                const datas = table.row($(event.currentTarget).parents('tr')).data();
                var oID = parseInt(datas.orderID);
                var oT = datas.orderType;
                if (oT === "寵物美容") {
                    // 處理寵物美容的邏輯
                } else if (oT === "寵物醫療") {

                } else {
                    // 處理其他類型的邏輯
                    try {
                        const form = document.createElement('form');
                        form.method = 'POST';
                        form.action = `Hotels/ShowBookingDetail`; // 目標 URI

                        // 將 payment 陣列轉換為 JSON 字串
                        // const ConvertID = JSON.stringify(bookingID);

                        // 創建一個隱藏的輸入元素來存放 JSON 字串
                        const input = document.createElement('input');
                        input.type = 'hidden';
                        input.name = 'BookingID'; // 你可以根據需要修改這個名稱
                        input.value = oID;

                        // 將輸入元素添加到表單中
                        form.appendChild(input);

                        // 將表單添加到文檔中並提交
                        document.body.appendChild(form);
                        form.submit(); // 提交表單
                    } catch (error) {
                        console.error('Error fetching booking history:', error);
                    }
                }
            });


        });
    },
    props: {
        userid: Number,
    },
    methods: {

        async getAllOrder() {
            try {
                const response = await axios.get(`api/UserInfoApi/UserOrder/${this.userid}`);
                this.userOrder = response.data;
                this.populateTable(this.userOrder);
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        populateTable(data) {
            const table = $('#orderTable').DataTable();
            table.clear();
            table.rows.add(data).draw();
        },
    },  
});

app.component('tab-相片集', {
    template: `#tab-Imgs`,
    data() {
        return {
            imgtypes: [],
            selectedtype: "",
            selectrdtypeimgs: [],
            simg: [],
            fileDatas:[]
        }
    },
    created() {
        this.fetchtype(this.userid);
    },
    props: {
        userid: Number,
    },
    methods: {
        async fetchtype(userid) {
            try {
                const response = await axios.get("api/KImg/GetImgType", {
                    params: {
                        ID: userid
                    }
                });
                this.imgtypes = response.data;
            } catch (error) {
                console.error('Error fetching pet types:', error);
            }
        },
        async postItype() {
            try {
                const formData = new FormData(this.$refs.form);
                formData.append('ID', this.userid);
                const response = await axios.post('api/KImg/postImgType', formData,{
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                $("#ITUpdateModal").modal("hide");
                console.log(JSON.stringify(response.data));
                this.fetchtype(this.userid);
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        getTypeCImg(pet) {
            return `/imgs/keeper_img/${pet}` ;
        },
        splittime(pet) {
            return pet.split("T")[0];
        },
        async cilckType(input) {
            this.selectedtype = "";
            this.selectedtype = input.id;
            try {
                const response = await axios.get("api/KImg/GetTypeAllImg", {
                    params: {
                        typeId: this.selectedtype
                    }
                });
                this.selectrdtypeimgs = response.data;
                // 強制刷新組件
                this.$forceUpdate();
            } catch (error) {
                console.error('Error fetching pet types:', error);
            }

        },
        async putname(input) {
            this.simg = [];
            this.simg = input;
            this.$forceUpdate();
            $("#ImgDetailModal").modal("show");
        },
        changeimgNane() {

        },
        handleFileChange(event) {
            const files = event.target.files;
            this.fileDatas = []; 
            for (const file of files) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.fileDatas.push(e.target.result); // 將每個文件的數據推入數組
                };
                reader.readAsDataURL(file);
            }
        },
        async postITImg() {
            const formdata = new FormData(this.$refs.ITIform);
            formdata.append("UserID", this.userid);
            formdata.append("type_name", this.imgtypes[this.selectedtype - 2].typename);
            formdata.append("img_name", "nonset");
            try {
            const response = await axios.post('api/KImg/UploadmuitiImg', formdata);
            console.log(response.data);
            try {
                    const response = await axios.get("api/KImg/GetTypeAllImg", {
                        params: {
                            typeId: this.selectedtype
                        }
                    });
                    this.selectrdtypeimgs = response.data;
                    // 強制刷新組件
                    this.$forceUpdate();
                } catch (error) {
                    console.error('Error fetching pet types:', error);
                }
            this.fetchtype(this.userid);
            $("#ITIUpdateModal").modal("hide");
            } catch(error) {
                console.error('Error fetching pet types:', error);
            }
            
        }
    }

});



app.mount("#app");