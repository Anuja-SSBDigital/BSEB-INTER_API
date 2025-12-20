using AdmitResultAPI.Model.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Security.Cryptography.Xml;

namespace AdmitResultAPI.Model
{
    public class InterPracticaladmitPdf : IDocument
    {
        private readonly List<StudentWithSubjectsDto> _data;
        private readonly byte[] _bsebLogo;
        private readonly Dictionary<int, byte[]> _Photos;  // Fixed: Use Dictionary to store photos
        private readonly Dictionary<int, byte[]> _Signatures;


        //public InterPracticaladmitPdf(List<StudentWithSubjectsDto> data)
        //{
        //    _data = data;
        //}
        //public InterPracticaladmitPdf(List<StudentWithSubjectsDto> data)
        //{
        //    _data = data;
        //    _bsebLogo = LoadImageFromUrl("https://intermediate.biharboardonline.com/Exam26/assets/img/bsebimage.jpg");
        //    //string imagePath = HttpContext.Current.Server.MapPath("~/Upload/bsebimage.jpg");
        //    //_bsebLogo = LoadImageFromFile(imagePath);




        //    // Initialize dictionaries for storing photos and signatures
        //    _Photos = new Dictionary<int, byte[]>();
        //    _Signatures = new Dictionary<int, byte[]>();

        //    // Load images for each student with fallback logic
        //    foreach (var student in _data)
        //    {
        //        // Construct the full URLs
        //        string photoUrl = $"https://intermediate.biharboardonline.com/Exam26/Uploads/StudentsReg/Photos/{student.Student.StudentPhotoPath}";
        //        string signatureUrl = $"https://intermediate.biharboardonline.com/Exam26/Uploads/StudentsReg/Signatures/{student.Student.StudentSignaturePath}";

        //        // Load photo and signature with fallback to the default BSEB logo if not found
        //        _Photos[student.Student.StudentID] = LoadImageFromUrlWithFallback(photoUrl);
        //        _Signatures[student.Student.StudentID] = LoadImageFromUrlWithFallback(signatureUrl);
        //    }
        //}

        public InterPracticaladmitPdf(List<StudentWithSubjectsDto> data)
        {
            _data = data;

            // Header logo only
            _bsebLogo = LoadImageFromFile("Assets/bsebimage.jpg");

            _Photos = new Dictionary<int, byte[]>();
            _Signatures = new Dictionary<int, byte[]>();

            foreach (var student in _data)
            {
                var photoPath = $"Uploads/Photos/{student.Student.StudentPhotoPath}";
                var signPath = $"Uploads/Signatures/{student.Student.StudentSignaturePath}";

                // PHOTO: fallback to BSEB logo
                //_Photos[student.Student.StudentID] = LoadImageFromFile(photoPath) ?? _bsebLogo;

                var photoBytes = LoadImageFromFile(photoPath);
                if (photoBytes != null)
                {
                    _Photos[student.Student.StudentID] = photoBytes;
                }

                // SIGNATURE: store ONLY if exists (no fallback)
                var signBytes = LoadImageFromFile(signPath);
                if (signBytes != null)
                {
                    _Signatures[student.Student.StudentID] = signBytes;
                }
            }
        }


        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            try
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().Column(col =>
                    {
                        foreach (var item in _data)
                        {
                            var student = item.Student;
                            var subjects = item.Subjects;


                            if (student.FacultyName == "VOCATIONAL")
                            {
                                // ================= HEADER =================
                                col.Item().Row(row =>
                                {
                                    //row.ConstantItem(70).Image("https://intermediate.biharboardonline.com/Exam26/assets/img/bsebimage.jpg", ImageScaling.FitWidth);
                                    row.ConstantItem(50).Image(_bsebLogo, ImageScaling.FitWidth);
                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("बिहार विद्यालय परीक्षा समिति").SemiBold().AlignCenter();

                                        c.Item().Text("BIHAR SCHOOL EXAMINATION BOARD").AlignCenter();

                                        c.Item().Text("INTERMEDIATE ANNUAL EXAMINATION, 2026")
                                            .AlignCenter();

                                        c.Item().Text("Admit Card For Practical Examination")
                                            .SemiBold().AlignCenter();
                                    });

                                    row.ConstantItem(90).AlignMiddle().Text($"FACULTY:\n{student.FacultyName}").FontSize(10).AlignCenter();
                                });

                                col.Item().PaddingVertical(10);

                                // ================= STUDENT DETAILS =================
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(3);
                                        c.RelativeColumn(1);
                                    });

                                    table.Cell().Border(1).Padding(6).Column(c =>
                                    {
                                        c.Item().Text($"BSEB UNIQUE Id     :   {student.UniqueNo}");
                                        c.Item().Text($"कॉलेज/+2 स्कूल का नाम:    {student.College}");
                                        c.Item().Text($"परीक्षार्थी का नाम:          {student.StudentName}");
                                        c.Item().Text($"पिता का नाम:         {student.FatherName}");
                                        c.Item().Text($"माता का नाम:         {student.MotherName}");
                                        c.Item().Text($"वैवाहिक स्थिति:         {student.MaritalStatus}");
                                        c.Item().Text($"परीक्षार्थी का आधार नं:         {student.AadharNo}");
                                        c.Item().Text($"दिव्यांग कोटि:         {student.Disability}");
                                        c.Item().Text($"परीक्षार्थी की कोटि:         {student.ExamTypeName}");
                                        c.Item().Text($"सूचीकरण संख्या/वर्ष:         {student.RegistrationNo}");
                                        c.Item().Text($"रौल कोड:              {student.RollCode}");
                                        c.Item().Text($"रौल क्रमांक:               {student.RollNumber}");
                                        c.Item().Text($"लिंग:                     {student.Gender}");
                                        c.Item().Text($"परीक्षा केंद्र का नाम:          {student.PracticalExamCenterName}");
                                    });

                                    table.Cell().Border(1).Padding(6).Column(c =>
                                    {
                                        // PHOTO (always exists because logo fallback)
                                        var photo = _Photos[student.StudentID];
                                        c.Item().Height(50).AlignCenter().AlignMiddle().Image(photo, ImageScaling.FitHeight);

                                        // SIGNATURE (draw only if available)
                                        if (_Signatures.TryGetValue(student.StudentID, out var signature))
                                        {
                                            c.Item().Height(30).AlignCenter().AlignMiddle().Image(signature, ImageScaling.FitHeight);
                                        }
                                        else
                                        {
                                            // Blank space for signature
                                            c.Item().Height(30);
                                        }
                                        // FitHeight ensures the image fits in the available space
                                    });
                                });


                                col.Item().PaddingVertical(10);

                                // ================= SUBJECT TABLE =================
                                col.Item().Text("प्रायोगिक परीक्षा के विषय (निर्धारित परीक्षा कार्यक्रम सहित)").SemiBold();

                                bool isVocationalFaculty = student.FacultyName?.Equals("VOCATIONAL", StringComparison.OrdinalIgnoreCase) == true;

                                bool hasVocationalFromDb = item.HasVocationalSubjects;

                                CategorizedSubjects cat = CategorizeSubjects(subjects, hasVocationalFromDb);

                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(1);
                                        c.RelativeColumn(3);
                                        c.RelativeColumn(2);
                                    });

                                    table.Header(h =>
                                    {
                                        h.Cell().Border(1).Padding(4).Text("विषय");
                                        h.Cell().Border(1).Padding(4).Text("कोड");
                                        h.Cell().Border(1).Padding(4).Text("नाम");
                                        h.Cell().Border(1).Padding(4).Text("तिथि");
                                    });

                                    // Compulsory
                                    foreach (var s in cat.Compulsory) DrawRow(table, "Compulsory", s);

                                    // Elective (max 3)
                                    foreach (var s in cat.Elective) DrawRow(table, "Elective", s);
                                    // ❌ HIDE when faculty = VOCATIONAL
                                    if (!isVocationalFaculty && cat.Additional != null) DrawRow(table, "Additional", cat.Additional);

                                    if (!isVocationalFaculty && cat.HasVocationalSubjects && cat.Vocational != null) DrawRow(table, "Vocational", cat.Vocational);

                                    //// Additional
                                    //if (cat.Additional != null) DrawRow(table, "Additional", cat.Additional);

                                    //// 🔴 Vocational ONLY if DB allows
                                    //if (cat.HasVocationalSubjects && cat.Vocational != null) DrawRow(table, "Vocational", cat.Vocational);
                                });
                                col.Item().PageBreak();
                                col.Item().PaddingVertical(10);

                                // ================= INSTRUCTIONS =================
                                col.Item().Text("परीक्षार्थी के लिए आवश्यक निदेश")
                                    .SemiBold()
                                    .Underline();

                                col.Item().Text(
                                    "1.प्रायोगिक परीक्षा दिनांक 10-01-2026 से 20-01-2026 तक संचालित होगी। केन्द्राधीक्षक दिनांक 10-01-2026 से 20-01-2026 तक की अवधि में परीक्षार्थीयों की संख्या के अनुसार तिथि एवं पाली का निर्धारण करके प्रायोगिक परीक्षा केन्द्र पर आवंटित सभी परीक्षार्थियों के प्रायोगिक विषयों की परीक्षा आयोजित करेंगें।\n" +
                                    "2.परीक्षार्थी अपने इस प्रवेश-पत्र में उल्लिखित प्रायोगिक परीक्षा केंद्रों पर दिनांक 10-01-2026 को पूर्वाह्न 09:00 बजे अनिवार्य रूप से जाकर परीक्षा केन्द्र के परिसर की सूचना पट्ट से यह जानकारी प्राप्त कर लेगें कि उनके द्वारा चयनित विषय की प्रायोगिक परीक्षा किस तिथि एवं किस पाली में संचालित होगी, जिसमें उन्हें सम्मिलित होना अनिवार्य है।\n" +
                                    "3. परीक्षार्थी के प्रत्येक प्रायोगिक विषय की परीक्षा के लिए 08 पृष्ठों की केवल एक ही उत्तरपुस्तिका मिलेगी। अतिरिक्त उत्तरपुस्तिका नहीं दी जाएगी। परीक्षार्थी उत्तरपुस्तिका लेते ही यह सुनिश्चित कर लें कि इसमें 8 पृष्ठ है एवं सही क्रम में है।\n" +
                                    "4.उत्तरपुस्तिका प्राप्त होते ही परीक्षार्थी अपने प्रवेश-पत्र तथा उत्तरपुस्तिका पर मुद्रित विवरणों (Details) का मिलान कर यह अवश्य सुनिश्चित हो लें कि जो उत्तरपुस्तिका परीक्षक द्वारा उन्हें दी गई है, वह उन्हीं की है। भिन्न विवरणों की उत्तरपुस्तिका प्राप्त होने पर उसे तुरंत परीक्षक को वापस लौटा दिया जाए। \n" +
                                    "5.उत्तरपुस्तिका प्राप्त होने पर परीक्षार्थी उनके आवरण पृष्ठ के पीछे अंकित 'परीक्षार्थियों के लिए निर्देश' अवश्य पढ़े एवं उसका अनुपालन करें। \n" +
                                    "6. परीक्षार्थी अपनी उत्तरपुस्तिका के कवर पृष्ठ के ऊपरी बायें तथा दायें भागों में क्रमांक-(1) में अपने उत्तर देने का माध्यम अंकित करते हुए क्रमांक-(2) में अपना पूर्ण हस्ताक्षर अंकित करें। इसके अलावा अन्य मुद्रित विवरणों में किसी भी प्रकार से कोई छेड़-छाड़ नहीं करें। \n" +
                                    "7. प्रायोगिक परीक्षा की उत्तरपुस्तिका के आवरण पृष्ठ के निचले बायें एवं दायें भागों को परीक्षार्थी द्वारा कदापि नहीं भरा जाएगा। अगर परीक्षार्थी इस भाग को भरते हैं, तो परीक्षार्थी का इस विषय में परीक्षाफल रद्द किया जा सकता है। ये दोनो भाग आंतरिक/बाह्य परीक्षकों को भरने के लिए दिया गया है। \n" +
                                    "8.उत्तरपुस्तिका के पन्नों के दोनो पृष्ठों पर तथा प्रत्येक लाइन पर लिखें एवं पृष्ठों को नष्ट न करें।\n" +
                                    "9. यदि रफ कार्य करने की आवश्यकता हो, तो परीक्षार्थी उत्तरपुस्तिका के अंतिम पृष्ठ पर रफ कार्य करके उसे काट दे/क्रॉस (x) कर दें।\r\n" +
                                    "10. उत्तरपुस्तिका के आंतरिक पृष्ठों पर दाहिने हाशिए में लाइन खींचकर सादा स्थान छोड़ रखा गया है। शेष स्थान रूल्ड है। परीक्षार्थी दाहिने हाशिए के सादे स्थान में कुछ भी नहीं लिखेंगें, चूँकि यह भाग परीक्षक के उपयोग के लिए है।\r\n" +
                                    "11. उत्तरपुस्तिका के पृष्ठों को मोड़े-फाड़े नहीं तथा बीच-बीच में व्यर्थ ही खाली न छोड़े।\n" +
                                    "12. प्रश्न-पत्र में दी हुई संख्या के अनुसार अपने उत्तरों की संख्या लिखें। \n" +
                                    "13.व्हाइटनर, ब्लेड तथा नाखून का इस्तेमाल करना सर्वथा वर्जित है, अन्यथा परीक्षाफल अमान्य कर दिया जाएगा।\r\n" +
                                    "14. प्रश्न्नोत्तर के समाप्त होने पर अंतिम में नीचे एक क्षैतिज रेखा खींच दें।\r\n" +
                                    "15.आंतरिक परीक्षक द्वारा उपलब्ध कराये गए उपस्थिति-पत्रक में परीक्षार्थी द्वारा यथा-स्तम्भ परीक्षा की तिथि अंकित करते हुए उत्तरपुस्तिका की क्रम संख्या लिखकर अपना हस्ताक्षर किया जाएगा। परीक्षार्थी की उपस्थित, अनुपस्थित एवं निष्कासन से संबंधित संगत गोले को नीले/काले पेन से परीक्षक द्वारा भरा जाएगा न कि परीक्षार्थी द्वारा।\n" +
                                    "16. परीक्षार्थी अपनी उत्तरपुस्तिका को आन्तरिक परीक्षक के पास जमा किये बिना परीक्षा भवन न छोड़े।\r\n" +
                                    "17. परीक्षा केन्द्र में कैलकुलेटर, मोबाइल फोन, इयर फोन, पेजर, ब्लूटूथ या इस प्रकार का कोई अन्य इलेक्ट्रॉनिक उपकरण ले जाना सख्त मना है।\r\n" +
                                    "18.जाँच परीक्षा में गैर-उत्प्रेषित या जाँच परीक्षा में अनुपस्थित छात्र/छात्रा इन्टरमीडिएट वार्षिक प्रायोगिक परीक्षा, 2026 में कदापि सम्मिलित नहीं हो सकते हैं।\r\n"

                                ).FontSize(4);

                                col.Item().PageBreak();
                            }
                            else
                            {
                                // ================= HEADER =================
                                col.Item().Row(row =>
                                {
                                    //row.ConstantItem(70).Image("https://intermediate.biharboardonline.com/Exam26/assets/img/bsebimage.jpg", ImageScaling.FitWidth);
                                    row.ConstantItem(50).Image(_bsebLogo, ImageScaling.FitWidth);
                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("बिहार विद्यालय परीक्षा समिति").SemiBold().AlignCenter();

                                        c.Item().Text("BIHAR SCHOOL EXAMINATION BOARD").AlignCenter();

                                        c.Item().Text("INTERMEDIATE ANNUAL EXAMINATION, 2026")
                                            .AlignCenter();

                                        c.Item().Text("Admit Card For Practical Examination")
                                            .SemiBold().AlignCenter();
                                    });

                                    row.ConstantItem(90).AlignMiddle().Text($"FACULTY:\n{student.FacultyName}").FontSize(10).AlignCenter();
                                });

                                col.Item().PaddingVertical(10);

                                // ================= STUDENT DETAILS =================
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(3);
                                        c.RelativeColumn(1);
                                    });

                                    table.Cell().Border(1).Padding(6).Column(c =>
                                    {
                                        c.Item().Text($"BSEB UNIQUE Id     :   {student.UniqueNo}");
                                        c.Item().Text($"कॉलेज/+2 स्कूल का नाम:    {student.College}");
                                        c.Item().Text($"परीक्षार्थी का नाम:          {student.StudentName}");
                                        c.Item().Text($"पिता का नाम:         {student.FatherName}");
                                        c.Item().Text($"माता का नाम:         {student.MotherName}");
                                        c.Item().Text($"वैवाहिक स्थिति:         {student.MaritalStatus}");
                                        c.Item().Text($"परीक्षार्थी का आधार नं:         {student.AadharNo}");
                                        c.Item().Text($"दिव्यांग कोटि:         {student.Disability}");
                                        c.Item().Text($"परीक्षार्थी की कोटि:         {student.ExamTypeName}");
                                        c.Item().Text($"सूचीकरण संख्या/वर्ष:         {student.RegistrationNo}");
                                        c.Item().Text($"रौल कोड:              {student.RollCode}");
                                        c.Item().Text($"रौल क्रमांक:               {student.RollNumber}");
                                        c.Item().Text($"लिंग:                     {student.Gender}");
                                        c.Item().Text($"परीक्षा केंद्र का नाम:          {student.PracticalExamCenterName}");
                                    });

                                    table.Cell().Border(1).Padding(6).Column(c =>
                                    {
                                        var photo = _Photos[student.StudentID];
                                        c.Item().Height(50).AlignCenter().AlignMiddle().Image(photo, ImageScaling.FitHeight);

                                        // SIGNATURE (draw only if available)
                                        if (_Signatures.TryGetValue(student.StudentID, out var signature))
                                        {
                                            c.Item().Height(30).AlignCenter().AlignMiddle().Image(signature, ImageScaling.FitHeight);
                                        }
                                        else
                                        {
                                            // Blank space for signature
                                            c.Item().Height(30);
                                        }  // FitHeight ensures the image fits in the available space
                                    });
                                });


                                col.Item().PaddingVertical(10);
                                // ================= SUBJECT TABLE =================
                                col.Item().Text("प्रायोगिक परीक्षा के विषय (निर्धारित परीक्षा कार्यक्रम सहित)").SemiBold();

                                bool isVocationalFaculty = student.FacultyName?.Equals("VOCATIONAL", StringComparison.OrdinalIgnoreCase) == true;

                                CategorizedSubjects cat = CategorizeSubjects(subjects, item.HasVocationalSubjects);

                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(1);
                                        c.RelativeColumn(3);
                                        c.RelativeColumn(2);
                                    });

                                    table.Header(h =>
                                    {
                                        h.Cell().Border(1).Padding(4).Text("विषय");
                                        h.Cell().Border(1).Padding(4).Text("कोड");
                                        h.Cell().Border(1).Padding(4).Text("नाम");
                                        h.Cell().Border(1).Padding(4).Text("तिथि");
                                    });

                                    // Compulsory
                                    foreach (var s in cat.Compulsory) DrawRow(table, "Compulsory", s);

                                    // Elective (max 3)
                                    foreach (var s in cat.Elective) DrawRow(table, "Elective", s);

                                    // Additional
                                    if (cat.Additional != null) DrawRow(table, "Additional", cat.Additional);

                                    // ✅ Show Vocational ONLY if student is NOT VOCATIONAL faculty AND DB allows
                                    if (!isVocationalFaculty && cat.HasVocationalSubjects && cat.Vocational != null)
                                    {
                                        DrawRow(table, "Vocational", cat.Vocational);
                                    }
                                });


                                col.Item().PageBreak();
                                col.Item().PaddingVertical(10);

                                // ================= INSTRUCTIONS =================
                                col.Item().Text("परीक्षार्थी के लिए आवश्यक निदेश")
                                    .SemiBold()
                                    .Underline();

                                col.Item().Text(
                                    "1.प्रायोगिक परीक्षा दिनांक 10-01-2026 से 20-01-2026 तक संचालित होगी। केन्द्राधीक्षक दिनांक 10-01-2026 से 20-01-2026 तक की अवधि में परीक्षार्थीयों की संख्या के अनुसार तिथि एवं पाली का निर्धारण करके प्रायोगिक परीक्षा केन्द्र पर आवंटित सभी परीक्षार्थियों के प्रायोगिक विषयों की परीक्षा आयोजित करेंगें।\n" +
                                    "2.परीक्षार्थी अपने इस प्रवेश-पत्र में उल्लिखित प्रायोगिक परीक्षा केंद्रों पर दिनांक 10-01-2026 को पूर्वाह्न 09:00 बजे अनिवार्य रूप से जाकर परीक्षा केन्द्र के परिसर की सूचना पट्ट से यह जानकारी प्राप्त कर लेगें कि उनके द्वारा चयनित विषय की प्रायोगिक परीक्षा किस तिथि एवं किस पाली में संचालित होगी, जिसमें उन्हें सम्मिलित होना अनिवार्य है।\n" +
                                    "3. परीक्षार्थी के प्रत्येक प्रायोगिक विषय की परीक्षा के लिए 08 पृष्ठों की केवल एक ही उत्तरपुस्तिका मिलेगी। अतिरिक्त उत्तरपुस्तिका नहीं दी जाएगी। परीक्षार्थी उत्तरपुस्तिका लेते ही यह सुनिश्चित कर लें कि इसमें 8 पृष्ठ है एवं सही क्रम में है।\n" +
                                    "4.उत्तरपुस्तिका प्राप्त होते ही परीक्षार्थी अपने प्रवेश-पत्र तथा उत्तरपुस्तिका पर मुद्रित विवरणों (Details) का मिलान कर यह अवश्य सुनिश्चित हो लें कि जो उत्तरपुस्तिका परीक्षक द्वारा उन्हें दी गई है, वह उन्हीं की है। भिन्न विवरणों की उत्तरपुस्तिका प्राप्त होने पर उसे तुरंत परीक्षक को वापस लौटा दिया जाए। \n" +
                                    "5.उत्तरपुस्तिका प्राप्त होने पर परीक्षार्थी उनके आवरण पृष्ठ के पीछे अंकित 'परीक्षार्थियों के लिए निर्देश' अवश्य पढ़े एवं उसका अनुपालन करें। \n" +
                                    "6. परीक्षार्थी अपनी उत्तरपुस्तिका के कवर पृष्ठ के ऊपरी बायें तथा दायें भागों में क्रमांक-(1) में अपने उत्तर देने का माध्यम अंकित करते हुए क्रमांक-(2) में अपना पूर्ण हस्ताक्षर अंकित करें। इसके अलावा अन्य मुद्रित विवरणों में किसी भी प्रकार से कोई छेड़-छाड़ नहीं करें। \n" +
                                    "7. प्रायोगिक परीक्षा की उत्तरपुस्तिका के आवरण पृष्ठ के निचले बायें एवं दायें भागों को परीक्षार्थी द्वारा कदापि नहीं भरा जाएगा। अगर परीक्षार्थी इस भाग को भरते हैं, तो परीक्षार्थी का इस विषय में परीक्षाफल रद्द किया जा सकता है। ये दोनो भाग आंतरिक/बाह्य परीक्षकों को भरने के लिए दिया गया है। \n" +
                                    "8.उत्तरपुस्तिका के पन्नों के दोनो पृष्ठों पर तथा प्रत्येक लाइन पर लिखें एवं पृष्ठों को नष्ट न करें।\n" +
                                    "9. यदि रफ कार्य करने की आवश्यकता हो, तो परीक्षार्थी उत्तरपुस्तिका के अंतिम पृष्ठ पर रफ कार्य करके उसे काट दे/क्रॉस (x) कर दें।\r\n" +
                                    "10. उत्तरपुस्तिका के आंतरिक पृष्ठों पर दाहिने हाशिए में लाइन खींचकर सादा स्थान छोड़ रखा गया है। शेष स्थान रूल्ड है। परीक्षार्थी दाहिने हाशिए के सादे स्थान में कुछ भी नहीं लिखेंगें, चूँकि यह भाग परीक्षक के उपयोग के लिए है।\r\n" +
                                    "11. उत्तरपुस्तिका के पृष्ठों को मोड़े-फाड़े नहीं तथा बीच-बीच में व्यर्थ ही खाली न छोड़े।\n" +
                                    "12. प्रश्न-पत्र में दी हुई संख्या के अनुसार अपने उत्तरों की संख्या लिखें। \n" +
                                    "13.व्हाइटनर, ब्लेड तथा नाखून का इस्तेमाल करना सर्वथा वर्जित है, अन्यथा परीक्षाफल अमान्य कर दिया जाएगा।\r\n" +
                                    "14. प्रश्न्नोत्तर के समाप्त होने पर अंतिम में नीचे एक क्षैतिज रेखा खींच दें।\r\n" +
                                    "15.आंतरिक परीक्षक द्वारा उपलब्ध कराये गए उपस्थिति-पत्रक में परीक्षार्थी द्वारा यथा-स्तम्भ परीक्षा की तिथि अंकित करते हुए उत्तरपुस्तिका की क्रम संख्या लिखकर अपना हस्ताक्षर किया जाएगा। परीक्षार्थी की उपस्थित, अनुपस्थित एवं निष्कासन से संबंधित संगत गोले को नीले/काले पेन से परीक्षक द्वारा भरा जाएगा न कि परीक्षार्थी द्वारा।\n" +
                                    "16. परीक्षार्थी अपनी उत्तरपुस्तिका को आन्तरिक परीक्षक के पास जमा किये बिना परीक्षा भवन न छोड़े।\r\n" +
                                    "17. परीक्षा केन्द्र में कैलकुलेटर, मोबाइल फोन, इयर फोन, पेजर, ब्लूटूथ या इस प्रकार का कोई अन्य इलेक्ट्रॉनिक उपकरण ले जाना सख्त मना है।\r\n" +
                                    "18.जाँच परीक्षा में गैर-उत्प्रेषित या जाँच परीक्षा में अनुपस्थित छात्र/छात्रा इन्टरमीडिएट वार्षिक प्रायोगिक परीक्षा, 2026 में कदापि सम्मिलित नहीं हो सकते हैं।\r\n"

                                ).FontSize(4);

                                col.Item().PageBreak();
                            }
                        }
                    
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {DateTime.Now:dd-MM-yyyy}");
                });
            }
            catch (Exception ex)
            {

                throw;
            }
       
        }

        private void DrawRow(TableDescriptor table, string type, SubjectPaperDto s)
        {
            try
            {
                       table.Cell().Border(1).Padding(4).Text(type);
            table.Cell().Border(1).Padding(4).Text(s.SubjectPaperCode?.ToString() ?? "");
            table.Cell().Border(1).Padding(4).Text(s.SubjectName ?? "");
            table.Cell().Border(1).Padding(4).Text("10/01/2026 To 20/01/2026");
            }
            catch (Exception ex)
            {

                throw;
            }
     
        }
        private static byte[] LoadImageFromUrlWithFallback(string imageUrl)
        {
            try
            {
                using var client = new HttpClient();
                var imageData = client.GetByteArrayAsync(imageUrl).Result;

                // If image is found, return the image bytes
                if (imageData != null && imageData.Length > 0)
                {
                    return imageData;
                }
                else
                {
                    // If image is not found, return null
                    return null;
                }
            }
            catch (Exception ex)
            {
                // If any error occurs (e.g., 404), return null
                return null;
            }
        }

        private static byte[] LoadImageFromFile(string fileName)
        {
            var root = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location)!;

            var fullPath = Path.Combine(root, fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException(fullPath);

            return File.ReadAllBytes(fullPath);
        }




        //private static byte[] LoadImageFromUrl(string imageUrl)
        //{
        //    try
        //    {
        //        using var client = new HttpClient();
        //        return client.GetByteArrayAsync(imageUrl).GetAwaiter().GetResult();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}

        private class CategorizedSubjects
        {
            public List<SubjectPaperDto> Compulsory { get; set; } = new();
            public List<SubjectPaperDto> Elective { get; set; } = new();
            public SubjectPaperDto? Additional { get; set; }
            public SubjectPaperDto? Vocational { get; set; }
            public bool HasVocationalSubjects { get; set; }
        }

        // ================= SUBJECT CATEGORIZATION =================
        private CategorizedSubjects CategorizeSubjects( List<SubjectPaperDto> subjects, bool hasVocationalFromDb)
        {
            try
            {
                var result = new CategorizedSubjects
                {
                    HasVocationalSubjects = hasVocationalFromDb
                };

                foreach (var s in subjects)
                {
                    string g = s.SubjectGroup ?? "";

                    if (g.StartsWith("Compulsory subject group-") || g.Contains("Compulsory"))
                        result.Compulsory.Add(s);
                    else if (g.Contains("Elective") || g.Contains("Optional"))
                        result.Elective.Add(s);
                    else if (g.Contains("Additional"))
                        result.Additional ??= s;
                    else if (g.Contains("Vocational"))
                        result.Vocational ??= s;
                }

                result.Elective = result.Elective.OrderBy(x => x.SubjectPaperCode ?? int.MaxValue).Take(3).ToList();

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
            
           
        }
    }
}
