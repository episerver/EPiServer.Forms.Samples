import React, { useEffect } from "react";
import axios from "axios";
import { ExposedDropdownMenu, TextField } from "@episerver/ui-framework";

const FormsCriteriaValue = (props: any) => {   
    const [fields, setFields] = React.useState([]);
    const [localModelState, setLocalModelState] = React.useState({});

    useEffect(() => {
        setLocalModelState(props.settings);
        if(props.settings.selectedForm) {
            getFieldsByFormId(props.settings.selectedForm);
        }
    }, []);

    const emitValueChange = (newState: any) => {
        setLocalModelState(newState);
        props.onValueChange(newState);
    };

    const getFieldsByFormId = (formGuid: any) => {
        axios.get("../EPiServer.Forms.Samples/FormInfo/GetElementFriendlyNames?formGuid=" + formGuid)
            .then(function (response) {
                console.log(response);
                setFields(response.data ?? []);
            })
            .catch(function (error) {
                console.error(error);
            });
    };

    return (
        <>     
            <div className="inline-criteria-editor">
                <div>
                    <ExposedDropdownMenu
                        label={props.editorConfig.selectedForm.label}
                        disabled={props.editorConfig.selectedForm.selectItems?.length === 0}
                        options={
                            props &&
                            props.editorConfig.selectedForm.selectItems.map((item) => {
                                return {
                                    label: item.text ?? "",
                                    value: item.value?.toString() ?? "",
                                };
                            })
                        }
                        value={localModelState.selectedForm}
                        onValueChange={(value) => {
                            if (localModelState.selectedForm === value) return;
                            let newState = { ...localModelState };
                            newState.selectedForm = value;
                            getFieldsByFormId(value);
                            emitValueChange(newState);
                        }}
                    />
                </div>
                <div>
                    <ExposedDropdownMenu
                        label={props.editorConfig.selectedField.label}
                        disabled={fields?.length === 0}
                        options={fields.map((item) => {
                            return {
                                label: item.friendlyName,
                                value: item.elementId?.toString(),
                            };
                        })}
                        value={localModelState.selectedField}
                        onValueChange={(value) => {
                            if (localModelState.selectedField === value) return;
                            let newState = { ...localModelState };
                            newState.selectedField = value;
                            emitValueChange(newState);
                        }}
                    />
                </div>
                <div>
                    <ExposedDropdownMenu
                        label={props.editorConfig.condition.label}
                        options={
                            props &&
                            props.editorConfig.condition.selectItems.map((item) => {
                                return {
                                    label: item.text ?? "",
                                    value: item.value?.toString() ?? "",
                                };
                            })
                        }
                        value={localModelState.condition?.toString()}
                        onValueChange={(value) => {
                            let newState = { ...localModelState };
                            newState.condition = value;
                            emitValueChange(newState);
                        }}
                    />
                </div>
                <div>
                    <TextField
                        outlined
                        label={props.editorConfig.value.label}
                        defaultValue={localModelState.value}
                        required={props.editorConfig.value.required}
                        onChange={(evt) => {
                            let newState = { ...localModelState };
                            newState.value = evt.currentTarget.value;
                            emitValueChange(newState);
                        }}
                    />
                </div>
            </div>
        </>
    );
};

export default FormsCriteriaValue;