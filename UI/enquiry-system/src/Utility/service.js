import Constant from "./constant";

const Service = {
    search: (param, onSuccess, onError) => {
        let options = {
            method: 'GET',
            headers: { "Access-Control-Allow-Origin": "*" },
            mode: 'cors',
            "Content-Type": undefined,
        }

        fetch(Constant.url.search + param.entity + '/' + param.id + '/' + param.timestamp, options)
            .then(response => {
                if (response.ok) {
                    return response
                }
                throw Error(response.statusText)
            })
            .then(response => response.json())
            .then(result => {
                onSuccess(result);
            })
            .catch(ex => onError(ex.message))
    },
    upload: (param, onSuccess, onError) => {
        let options = {
            method: 'POST',
            headers: { "Access-Control-Allow-Origin": "*" },
            mode: 'cors',
            "Content-Type": undefined,
            body: param,
        }

        fetch(Constant.url.csvUpload, options)
            .then(
                response => response.json())
            .then(result => {
                onSuccess(result);
            })
            .catch(ex => { onError(ex.message)})
    },
    clearCollection : (onSuccess, onError) => {
        let options = {
            method: 'DELETE',
            headers: { "Access-Control-Allow-Origin": "*" },
            mode: 'cors',
            "Content-Type": undefined,
        }

        fetch(Constant.url.clearCollection, options)
            .then(
                response => response.json())
            .then(result => {
                onSuccess(result);
            })
            .catch(ex => { onError(ex.message)})
    }

    
}

export default Service;