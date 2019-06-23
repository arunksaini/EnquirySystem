import React from 'react';
import './App.css';
import CsvUploader from '../csv-uploader';
import Search from '../search';
import 'bootstrap/dist/css/bootstrap.min.css';
import Loader from '../loader'
import Modal from '../modal'
import Tabs from 'react-bootstrap/Tabs'
import Tab from 'react-bootstrap/Tab'

class App extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      modalMessage: '',
      showModal: false,
      showLoader: false,
    }

  };

  onHideModal = () => this.setState({ showModal: false, modalMessage: '' });
  onShowModal = (message) => this.setState({ showModal: true, modalMessage: message });
  onShowLoader = (show) => this.setState({ showLoader: show });

  render() {
    return (

      <div className="App container-fluid">

        <h1>Enquiry System</h1>
        
        <Tabs >
          <Tab eventKey="UploadData" title="Upload Data">
          <CsvUploader showModal={this.onShowModal} showLoader={this.onShowLoader} ></CsvUploader>
          </Tab>
          <Tab eventKey="Query" title="Query">
          <Search showModal={this.onShowModal} showLoader={this.onShowLoader} />
          </Tab>
        </Tabs>
        <br/>
        <div class="row">
          <div class="col" >
            {this.state.showLoader ? <Loader /> : null}
          </div>
        </div>
        <Modal message={this.state.modalMessage} show={this.state.showModal} handleClose={this.onHideModal} />
      </div>
    );
  }
}



export default App;
