// Function to upload images
$('#uploadForm').submit(function (e) {
    e.preventDefault();

    // Покажите оверлей с анимацией загрузки
    $('#loadingOverlay').show();

    let formData = new FormData(this);
    formData.append('sessionkey', $('#sessionKey').val());  // Append session key

    $.ajax({
        url: 'api/Main/add_image_to_event/1',
        type: 'POST',
        data: formData,
        success: function (data) {
            // Скройте оверлей после успешного запроса
            $('#loadingOverlay').hide();
            alert('Images uploaded successfully');
        },
        error: function () {
            // Скройте оверлей при ошибке запроса
            $('#loadingOverlay').hide();
            alert('Error uploading images');
        },
        cache: false,
        contentType: false,
        processData: false
    });
});

// Function to perform face search
$('#faceSearchForm').submit(function (e) {
    e.preventDefault();

    let formData = new FormData(this);

    $.ajax({
        url: 'api/Main/search_face/4',
        type: 'POST',
        data: formData,
        success: function (data) {
            // Display matched images
            let matches = data.matches;
            $('#results').empty();
            matches.forEach(function (imagePath) {
                $('#results').append('<img src="' + imagePath + '" alt="Matched Face" class="img-thumbnail">');
            });
        },
        cache: false,
        contentType: false,
        processData: false
    });
});
