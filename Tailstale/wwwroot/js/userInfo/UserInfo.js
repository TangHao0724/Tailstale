/*
1.建立側欄切煥功能
2. 顯示使用者資訊，並且使用model去修改內容。
3. 新增寵物的方法
4. 歷史貼文
*/

const app = Vue.createApp({
    data() {
        return {
            currentTab: 'Home',
            tabs: ['主頁', '使用者資訊', '寵物資訊','歷史訂單','歷史貼文','相片集'],
        }
    },
    computed: {
        currentTabComponent() {
            return `tab-${this.currentTab}`;
        }
    },
    methods: {

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
            user : null ,
        }
        
    },
    created() {
        this.fetchUserInfo(this.userid); // 替換成你想要查詢的用戶 ID
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
        }
    },
});

app.component('tab-寵物資訊', {
    template: `#tab-PInfo`,
    data() {
        return {
            pet_types: [],
        }
    },
    created() {
        this.fetchPetTypes();
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
        submitForm() {
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
            data.keeper_ID = this.keeperId;
            axios.post('api/UserInfoApi/PostPetInfo', data, {
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(response => {
                console.log(response.data);
            })
            .catch(error => {
                console.error('Error:', error);
            });
        }
    }

        


    
});

app.component('tab-歷史訂單', {
    template: `#tab-HisOrder`
});

app.component('tab-歷史貼文', {
    template: `#tab-HisPost`
});

app.component('tab-相片集', {
    template: `#tab-Imgs`
});



app.mount("#app");