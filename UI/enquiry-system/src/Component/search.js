import React from 'react';
import Service from '../Utility/service';


class search extends React.Component {
   constructor(props) {
      super(props);

      this.state = {
         entity: 'Product',
         id: 1,
         timestamp: 1484731172,
         output: ''
      }

   };

   validateInputs = () => {
      let status = false;
      let entity = this.state.entity.toLowerCase();
      let id = this.state.id;
      let timestamp = this.state.timestamp;

      if (entity && id && timestamp) {
         if (entity === 'product' || entity === 'invoice' || entity === 'order') {
            let message = '';
            if (id.isInteger) {
               message += 'Id must be a number.'
            }
            if (timestamp.isInteger) {
               message += 'Timestamp must be a number.'
            }
            if (message) {
               this.props.showModal(message);
            }
            else {
               status = true;
            }

         }
         else {
            this.props.showModal('Please search on one of the following vaid entities. Product or Invoice or Order');
         }
      }
      else {
         this.props.showModal('Please fill all mandatory fields : Entity  Id & Timestamp');
      }

      return status;
   }


   onSearchSuccess = (result) => {
      this.props.showLoader(false);
      let message = '{}'
      if (result) {
         message = JSON.stringify(result);
      }
      this.setState({ output: message })
   };
   onSearchFail = (result) => {
      this.props.showLoader(false);
      this.props.showModal(result);
   };

   onTimestampChange = (event) => this.setState({ timestamp: event.target.value, output: '' });
   onIdChange = (event) => this.setState({ id: event.target.value, output: '' });
   onEntityChange = (event) => { debugger; this.setState({ entity: event.target.value, output: '' }); }

   handleSearch = (event) => {

      if (this.validateInputs()) {
         let param = {
            entity: this.state.entity,
            id: this.state.id,
            timestamp: this.state.timestamp
         }
         this.props.showLoader(true);
         Service.search(param, this.onSearchSuccess, this.onSearchFail);
      }
   }


   render() {
      return (
         <React.Fragment>
            <h3>Query</h3>

            <div class="row">
            <div class="col">
               </div>
               <div class="col">
                  Entity 
               </div>
               <div class="col">
                  <select onChange={this.onEntityChange} className="form-control">
                     <option value="Product">Product</option>
                     <option value="Invoice">Invoice</option>
                     <option value="Order">Order</option>
                  </select>
               </div>
               <div class="col">
               </div>
            </div>


            <div class="row">
            <div class="col">
               </div>
               <div class="col">
               <label for="Id">Id</label>   
               </div>
               <div class="col">
                  <input className="form-control" id="Id"
                     placeholder='Id...'
                     value={this.state.id}
                     onChange={this.onIdChange}
                     title='A number grater than 0'
                    
                  />
               </div>
               <div class="col">
               </div>
            </div>

            <div class="row">
            <div class="col">
               </div>
               <div class="col">
                  timestamp 
               </div>
               <div class="col">
                  <input  className="form-control"
                     placeholder='Timestamp...'
                     value={this.state.timestamp}
                     onChange={this.onTimestampChange}
                     title='A number for timestamp'
                    
                  />
               </div>
               <div class="col">
               </div>
            </div>
<br/>
            <div class="row">
               <div class="col">
                  <button type="button"
                     className="btn btn-primary"
                     text=''
                     onClick={this.handleSearch}
                  >Search</button>
               </div>
            </div>
            <br/>
            <div class="row">
               <div class="col">
                  {this.state.output}
               </div>
            </div>

         </React.Fragment>
      );
   }
}






export default search;