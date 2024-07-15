function DragDropImg(yourInputFile) {
    //document.querySelector('#roomImg_URL')
    let input = yourInputFile;
    let files = [];
    var imgbox = document.querySelector('.imgbox');
    var container = document.querySelector('.containerbox');
    var text = document.querySelector('.inner');
    var browse = document.querySelector('.select');
    let hasFileFromDrag = false;

    browse.addEventListener('click', e => {
        e.preventDefault();
        e.stopPropagation()
        //if (!hasFileFromDrag) {
        //    input.click();
        //}
        input.click();
    });
    //let newFiles;

    input.addEventListener('change', () => {
        newFiles = Array.from(input.files);
        newFiles = newFiles.filter(file => !files.includes(file));
        files = files.concat(newFiles);

        showImages();

    });
    const showImages = () => {
        let images = '';
        files.forEach((e, i) => {
            images += `<div class="image">
                                            <img src="${URL.createObjectURL(e)}" alt="">
                                            <span onclick="delImage(${i})">&times;</span>
                                        </div>`;
        });

        container.innerHTML = images;
    };
    const delImage = index => {
        files.splice(index, 1);
        let removeFileList = new DataTransfer();
        files.forEach(file => removeFileList.items.add(file));
        input.files = removeFileList.files;
        showImages();
    }
    imgbox.addEventListener('dragover', e => {
        e.preventDefault();
        e.stopPropagation();
        imgbox.classList.add('dragover');
        text.innerHTML = "放手圖片就上傳";
    });
    imgbox.addEventListener('dragleave', e => {
        e.preventDefault()
        e.stopPropagation();
        imgbox.classList.remove('dragover');
        text.innerHTML = '圖片放這裡<span class="select">瀏覽</span>';
        browse = document.querySelector('.select');
    });
    imgbox.addEventListener('drop', e => {
        e.preventDefault();
        e.stopPropagation();
        imgbox.classList.remove('dragover');
        text.innerHTML = '圖片放這裡<span class="select">瀏覽</span>';
        browse = document.querySelector('.select');



        let existingFiles = Array.from(input.files);
        let uploadFiles = Array.from(e.dataTransfer.files);

        // 合併新上傳的檔案至 files 陣列
        let allFiles = existingFiles.concat(uploadFiles);

        // 創建新的 DataTransfer 物件並設定 files
        let dataTransfer = new DataTransfer();
        allFiles.forEach(file => dataTransfer.items.add(file));

        // 將新的 DataTransfer 物件設定給 input 的 files 屬性
        input.files = dataTransfer.files;
        files = Array.from(input.files);
        // 顯示圖片列表
        showImages();
       // input.dispatchEvent(new Event('change'));
       // hasFileFromDrag = true;
    });

}
