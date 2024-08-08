var App = Vue.createApp({
    data() {
        return {
            imgurlLu: "",
            seridm: "",
        }
    },
    methods: {
        async GetUserimgm(userid) {
            try {
                //傳入ID，如果
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

    },
})

app.mount('#app');

