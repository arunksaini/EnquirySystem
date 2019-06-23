import React from 'react';
import Service from '../Utility/service';

const uploader = (props) => {
  let validateFileType = (fileName) => {

    var regex = /^.+\.(txt|csv)$/;
    if (regex.test(fileName.toLowerCase())) {
      return true;
    }
    else
      return false;
  }

  let onSuccess = (result) => {
    props.showLoader(false);
    let message = ''
    if (result) {
      message = JSON.stringify(result);
    }
    props.showModal(message);
  };
  let onFail = (result) => {
    props.showLoader(false);
    props.showModal(result);
  };

  let handleUpload = (event) => {
    if (event.target.files[0]) {
      if (validateFileType(event.target.files[0].name)) {
        let formData = new FormData();
        formData.append('file', event.target.files[0]);
        props.showLoader(true);
        Service.upload(formData, onSuccess, onFail);
        event.target.value = '';
      }
      else {
        props.showModal('Please upload a valid file type. i.e. csv or txt');
      }
    }
  };

  let onClearCollection = () =>{
    props.showLoader(true);
    Service.clearCollection(onSuccess, onFail);
  };

  return (
    <React.Fragment>
      <h3>Upload Data </h3>
      <input
        type="file"
        name="file"
        onChange={handleUpload}
        accept=".csv"
        style = {{width :"100px"}}

      />
      <br/>
      <br/>
      <button onClick={onClearCollection}  className="btn btn-danger"> Clear Collection </button>
    </React.Fragment>
  );

}

export default uploader;