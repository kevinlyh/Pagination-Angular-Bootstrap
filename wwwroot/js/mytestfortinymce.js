//20180327 Yanhua Liu  testing upload images of TinyMCE

tinymce.init({
    selector: 'textarea',
    width: 760,
    height: 500,
    plugins: 'link lists table image code',
    autoresize_min_height: 350,
    //toolbar: 'undo redo | image code',
    paste_data_images: true,

    // without images_upload_url set, Upload tab won't show up
    images_upload_url: '/Home/UploadFile',
    images_upload_base_path: '/',
    // we override default upload handler to simulate successful upload
    images_upload_handler: mceHandle,

    init_instance_callback: function (ed) {
        //ed.execCommand('mceImage');
    }
});


var _$http;
var _$scope;
var app = angular.module('myApp', []);
app.controller('myCtrl', function ($scope, $http, $timeout) {
    _$http = $http;
    _$scope = $scope;
    $scope.buttonValue = "Edit";
    $scope.editState = false;
    $scope.toggleEdit = function () {
        if ($scope.editState) {
            setContent();
            $('#contentDisp').html(tinymce.get('myTestForTinymce').getContent());
            $timeout(toggleState, 500);
            //toggleState();
            return;
        }
        $scope.editState = !$scope.editState;
        $scope.buttonValue = $scope.editState ? "Save" : "Edit";
    };
    
    $scope.cancelEdit = function () {
        $scope.editState = false;
        tinymce.get('myTestForTinymce').setContent($('#contentDisp').html());
        _$scope.buttonValue = _$scope.editState ? "Save" : "Edit";
    };
    getContactContent();
});

function toggleState() {
    getContactContent();
    _$scope.editState = !_$scope.editState;
    _$scope.buttonValue = _$scope.editState ? "Save" : "Edit";
}

function getContactContent() {
    var url = "/Home/LoadContactContent?filename=Contact";
    _$http.get(url).success(function (responseObject) {
        //console.log(responseObject);
        $('#contentDisp').html(responseObject);
        tinymce.get('myTestForTinymce').setContent(responseObject);
    });
}

function setContent() {
    var url = "/Home/SaveContactContent";
    var content = tinymce.get('myTestForTinymce').getContent();
    var formData;
    formData = new FormData();
    formData.append('content', content);
    formData.append('filename', "Contact");
    _$http({
        url: url,
        method: "POST",
        data: formData,
        headers: { 'Content-Type': undefined }
    }).success(function (response) {
        //console.log(response);
    }).error(function () {
        console.log("error.");
    });
}

function mceHandle(blobInfo, success, failure) {
    //console.log("data:" + blobInfo.blob().type + ";base64," + blobInfo.base64());
    //console.log(blobInfo.blob());
    var formData;
    formData = new FormData();
    formData.append('file', blobInfo.blob());
    formData.append('target', 'contact');
    _$http({
        url: '/Home/UploadFile',
        method: "POST",
        data: formData,
        headers: { 'Content-Type': undefined }
    }).success(function (response) {
        //console.log(response);
        success("http://"+response);
        }).error(function () {
            console.log("error.");
        });
    //success('http://moxiecode.cachefly.net/tinymce/v9/images/logo.png');
};