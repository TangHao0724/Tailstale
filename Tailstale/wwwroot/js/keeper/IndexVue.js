const app = Vue.createApp({
    data() {
        return {
            imgurlLu: "",
            useridm: "",
            usertype:"",
            fileDatas: []
        }
    },
    methods: {
        async GetUserimgm(userid) {
            if (this.usertype != 0) {

            }
            try {
                const formData = {
                    'UserID': userid, // 使用傳遞的 userid
                    'type_name': userid + '_head',
                    "img_name": "head",
                };

                const response = await axios.post('api/KImg/GetSingleImg', formData);
                console.log(JSON.stringify(response.data));
                this.imgurlLu = `/imgs/keeper_img/${response.data.img_url}`;
            } catch (error) {
                console.error('Error fetching user info:', error);
            }
        },
        inputFile() {
            $("#updateImg").click();
        },
        handleFileChange() {
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
        async postarticle() {
            try {
                let formdata = new FormData(this.$refs.postform);
                if (this.usertype == 0) {
                    formdata.append('Keeper_ID', this.useridm);
                    const response = axios.post("api/social/PostArticle", formdata);
                    alert(response.data);
                } else {
                    formdata.append('Business_ID', this.useridm);
                    const response = axios.post("api/social/PostArticle", formdata);
                    alert(response.data);
                }
            } catch(error) {
                console.error('Error fetching user info:', error);
            }
        }
    },
    mounted() {
        this.useridm = $("#user-id").data('user-id');
        this.usertype = $("#user-id").data("user-type");
        if (this.usertype == 0) {
            this.GetUserimgm(this.useridm); // 在掛載時調用方法
        }

    }
});
app.mount("#app");
