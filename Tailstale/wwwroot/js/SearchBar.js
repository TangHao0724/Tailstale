$(document).ready(function () {
    document.querySelectorAll('.searchCookie').forEach(card => {
        function updateShowCatDog() {
            const catCount = parseInt($('#Cat').val()) || 0;
            const dogCount = parseInt($('#Dog').val()) || 0;
            $('#showCatDog').val(`${catCount}貓 ${dogCount}狗`);
        }

        $('.ctrl__button--increment').on('click', function () {
            const input = $(this).siblings('.ctrl__counter').find('.ctrl__counter-input');
            let value = parseInt(input.val()) || 0;
            if (value < 5) {
                value++;
                input.val(value);
                input.siblings('.ctrl__counter-num').text(value);
                updateShowCatDog();
            }
        });

        $('.ctrl__button--decrement').on('click', function () {
            const input = $(this).siblings('.ctrl__counter').find('.ctrl__counter-input');
            let value = parseInt(input.val()) || 0;
            if (value > 0) {
                value--;
                input.val(value);
                input.siblings('.ctrl__counter-num').text(value);
                updateShowCatDog();
            }
        });

        // 初始化顯示
        updateShowCatDog();

    });
   
    // 點擊 showCatDog 按鈕顯示或隱藏 cookie-card
    $('#showCatDog').on('click', function () {
        $('.cookie-card').toggle();
    });

    // 點擊 accept 按鈕隱藏 cookie-card
    $('.accept').on('click', function () {
        $('.cookie-card').hide();
    });

    let cI = ""; // 外部作用域的 startDate
    let cO = ""; // 外部作用域的 endDate

    $('input.dateRange').on('apply.daterangepicker', function (ev, picker) {
        // 獲取新選取的日期
        const newStartDate = picker.startDate.format('YYYY-MM-DD');
        const newEndDate = picker.endDate.format('YYYY-MM-DD');

        // 如果沒有變更，使用目前選取的日期
        if (!$(this).data('startDate') || !$(this).data('endDate')) {
            cI = newStartDate; // 設置外部的 cI
            cO = newEndDate;   // 設置外部的 cO
        } else {
            // 獲取目前所選的日期
            const currentStartDate = $(this).data('startDate');
            const currentEndDate = $(this).data('endDate');

            // 檢查日期是否有變更
            if (newStartDate !== currentStartDate || newEndDate !== currentEndDate) {
                cI = newStartDate; // 設置外部的 cI
                cO = newEndDate;   // 設置外部的 cO

                // 更新目前所選的日期
                $(this).data('startDate', newStartDate);
                $(this).data('endDate', newEndDate);
            } else {
                // 使用目前的日期
                cI = currentStartDate;
                cO = currentEndDate;
            }
        }

        // 觸發 onchange 事件
        $(this).trigger('onchange');

        // 可選：在這裡調用其他函數
        // otherFunction(cI, cO);
    });





    //$('input.dateRange').on('apply.daterangepicker', function (ev, picker) {
    //    cI = picker.startDate.format('YYYY-MM-DD'); // 設置外部的 cI
    //    cO = picker.endDate.format('YYYY-MM-DD');   // 設置外部的 cO

    //    // 觸發 onchange 事件
    //    $(this).trigger('onchange');
        
    //    // 可選：在這裡調用其他函數
    //    // otherFunction(cI, cO);
    //});

    $("#bt").on("click", async function () {
        console.log(cI);

        var Cat = document.getElementById("Cat").value;
        var Dog = document.getElementById("Dog").value;
        var destination = document.getElementById("destination").value;
        const dtCI = new Date(cI);

        let checkIndate = dtCI.getFullYear() + "-" +
            String(dtCI.getMonth() + 1).padStart(2, '0') + "-" +
            String(dtCI.getDate()).padStart(2, '0');

        const dtCO = new Date(cO);

        let checkOutdate = dtCO.getFullYear() + "-" +
            String(dtCO.getMonth() + 1).padStart(2, '0') + "-" +
            String(dtCO.getDate()).padStart(2, '0');

        console.log(checkOutdate);
        // console.log(dtCI);
        // console.log(dtCO);

        if (checkIndate <= checkOutdate && checkIndate != null && checkOutdate != null) {
            if (destination != "") {
                if (Cat != null && Cat > 0 && Dog != null && Dog > 0) {
                    window.location.href = `/Hotels/SearchHotels?startDate=${checkIndate}&endDate=${checkOutdate}&Cat=${Cat}&Dog=${Dog}&addressorname=${destination}`;
                }
                else if (Cat != null && Cat > 0) {
                    window.location.href = `/Hotels/SearchHotels?startDate=${checkIndate}&endDate=${checkOutdate}&Cat=${Cat}&addressorname=${destination}`;
                }
                else if (Dog != null && Dog > 0) {
                    window.location.href = `/Hotels/SearchHotels?startDate=${checkIndate}&endDate=${checkOutdate}&Dog=${Dog}&addressorname=${destination}`;
                }
                else {
                    window.location.href = `/Hotels/SearchHotels?startDate=${checkIndate}&endDate=${checkOutdate}&addressorname=${destination}`;
                }

            }
            else {
                if (Cat != null && Cat > 0 && Dog != null && Dog > 0) {
                    window.location.href = `/Hotels/SearchHotels?startDate=${checkIndate}&endDate=${checkOutdate}&Cat=${Cat}&Dog=${Dog}`;
                }
                else if (Cat != null && Cat > 0) {
                    window.location.href = `/Hotels/SearchHotels?startDate=${checkIndate}&endDate=${checkOutdate}&Cat=${Cat}`;
                }
                else if (Dog != null && Dog > 0) {
                    window.location.href = `/Hotels/SearchHotels?startDate=${checkIndate}&endDate=${checkOutdate}&Dog=${Dog}`;
                }
                else {
                    window.location.href = `/Hotels/SearchHotels?startDate=${checkIndate}&endDate=${checkOutdate}`;
                }
            }
           

        }
        else {
            alert("請確認輸入資料使否錯誤");
        }




    });

});