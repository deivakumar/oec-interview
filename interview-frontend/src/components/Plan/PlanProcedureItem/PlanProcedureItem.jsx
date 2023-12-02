import React, { useEffect, useState } from "react";
import ReactSelect from "react-select";
import {
    addUserToProcedure
} from "../../../api/api";

const PlanProcedureItem = ({ planId, procedure, users, procedureUsers }) => {
    const [selectedUsers, setSelectedUsers] = useState(null);

    useEffect(() => {
        if(procedureUsers?.length){
            var selectedUserOptions = [];
            procedureUsers.map((u) => selectedUserOptions.push({ label: u.user.name, value: u.userId }));
            setSelectedUsers(selectedUserOptions);
        }        
    }, []);

    const handleAssignUserToProcedure = async (e) => {
        setSelectedUsers(e);
        await addUserToProcedure(planId, procedure.procedureId, e.map(p => p.value));
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
