const vueApp = {
    data() {
        return {
            baseAddress: "https://localhost:7112/Hotels/",
            roomList: [],
            reviewRating: 0,
            reviewText: '',
            bookingID: 0, // 初始值設為 0
        };
    },
    mounted() {
        const urlParams = new URLSearchParams(window.location.search);
        const bookingID = urlParams.get('bookingID'); // 從 URL 查詢參數中獲取
        this.bookingID = bookingID ? parseInt(bookingID) : 0; // 確保轉換為整數，若未定義則設為 0
        this.getRoomList(); // 在組件掛載時獲取房間列表
    },
    methods: {
        getRoomList() {
            try {
                var request = {};
                request.bookingID = this.bookingID;
                axios.post(`${this.baseAddress}GetRoomList`, request)
                    .then(response => {
                        //alert(JSON.stringify(response.data));
                        this.roomList = response.data;

                        this.roomList.forEach(room => {
                            this.setdata(room);
                        });
                    })
                    .catch(err => { alert(err) })
                // const response =  axios.post(`${this.baseAddress}GetRoomList`,this.bookingID);
                // this.roomList = response.data;
            } catch (err) {
                alert(err);
            }
        },
        setdata(room) {
            Vue.set(this, `${room.roomID}_content`, '');
            Vue.set(this, `${room.roomID}_rating`, 0);
        },
        setRating(roomID, star) {
            const room = this.roomList.find(r => r.roomID === roomID);
            if (room) {
                room.rating = star;
            }
            //alert(room.reviewRating);
            this.reviewRating = room.rating;
            // alert(room.rating);
        },

        submitReview() {
            const review = this.roomList.map(room => {
                return {
                    roomID: room.roomID,
                    bookingID: this.bookingID,
                    reviewRating: this.reviewRating,
                    reviewText: this.reviewText,
                };
            });
            // alert(JSON.stringify(review));
            axios.post(`${this.baseAddress}CreateReview`, review)
                .then(response => {
                    //alert("123");
                    //alert(JSON.stringify(response.data));
                    // this.roomList = response.data;


                })
                .catch(err => { alert(err) })
            // fetch(`${this.baseAddress}CreateReview`, {
            //     method: 'POST',
            //     headers: {
            //         'Content-Type': 'application/json',
            //     },
            //     body: JSON.stringify(review), // 將所有評論一起發送
            // })
            //     .then(response => {
            //         if (!response.ok) {
            //             throw new Error('提交評論失敗: ' + response.statusText);
            //         }
            //         return response.json();
            //     })
            //     .then(data => {
            //         // 成功處理
            //         this.reviewRating = 0;
            //         this.reviewText = '';
            //         console.log('評論提交成功:', data);
            //     })
            //     .catch(err => {
            //         console.error('發生錯誤:', err);
            //     });
        },

    },
};


Vue.createApp(vueApp).mount("#app");
  
                