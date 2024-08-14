/*
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
            tabs: ['主頁', '使用者資訊', '寵物資訊', '歷史訂單', '歷史貼文', '相片集',],
            useridm:""
        }
    },
    computed: {
        currentTabComponent() {
            return `tab-${this.currentTab}`;
        }
    },
    methods: {
        async GetUserimgm(userid) {
            try {
                const formData = {
                    'UserID': this.useridm,
                    'type_name': this.useridm + '_head',
                    "img_name": "head",
                };

                const response = await axios.post('api/KImg/GetSingleImg', formData);
                console.log(JSON.stringify(response.data));
                this.imgurlLu = `/imgs/keeper_img/${response.data.img_url}`;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
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
        this.GetUserimgm(this.useridm);
    }
});


app.component('tab-主頁', {
    template: `#tab-Main`,
    data() {
        return {

        }
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
        //更新會員資料
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
                alert(JSON.stringify(response.data));
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
    template: `#tab-HisOrder`
});

app.component('tab-歷史貼文', {
    template: `#tab-HisPost`,
    data() {
        return {
            mal:"",
        }
    },
    created() {

    },
    props: {
        userid: Number,
    },
    methods: {

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
                alert(JSON.stringify(response.data));
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
            alert("aa");
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