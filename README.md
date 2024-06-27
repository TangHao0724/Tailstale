## 編輯事項
1. 先不要更動HOMEController，到第一次合併前在討論展示方法。
2. 如果要建立連接到自己的CONTROLLER，在Shared/_Layout.cshtml中添加一個按鈕引導自己功能頁面，
如有其他需求，添加在自己的頁面、別再NVABAR。
3. 要添加新controller，在Partial/TailstaleContext.cs中下方添加:
EX；「public DbSet<Tailstale.Models.{{選擇要的Model}}> 選擇要得Model { get; set; } = default!; 」
並依造標準的EF CORE 建立CRUD並修改內容即可。

4. 如果建立viewmodel，請個別建立"{{負責部分}}_viewmodel"資料夾。
5. 目前暫時有靜態圖片需求，請各自在wwwroot/libs內各自"{{負責部分}}_img"建立資料夾並放入圖片。
6. CSS、JS則相同，各自命名"{{負責部分}}_styles"建立資料夾並放入。

7. 如果有任何問題，直接問群組。或者GPT。



(負責部分EX：Hotel/Salon/Hospital/med-record/Socal/Index)
