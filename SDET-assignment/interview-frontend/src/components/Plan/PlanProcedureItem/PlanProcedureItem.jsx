import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({ planProcedure, users, handleAssignUserToProcedure}) => {
    const [selected, setSelected] = useState([]);    

    useEffect(() => { 
        console.log("use effect trigger");        
        
        var planProcedureUsers = planProcedure?.planProcedureUsers?.map(function(u) { return {value: u.userId, label: u.name }; });
        setSelected(planProcedureUsers);
        return () => {
            console.log("use effect trigger return");
          };
    }, []);

    const handleOnChange = (e) => {
        var userIds = e.map(function(u) { return u.value; });       
        setSelected(e);
        handleAssignUserToProcedure(planProcedure, userIds);

        console.log(e);
    };

    return (
        <div className="py-2">
            <div>
                {planProcedure.procedure.procedureTitle}
            </div>
            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}               
                onChange={(e) => handleOnChange(e)}
                options={users}
                value={selected}
            />
        </div>
    );
};

export default PlanProcedureItem;
