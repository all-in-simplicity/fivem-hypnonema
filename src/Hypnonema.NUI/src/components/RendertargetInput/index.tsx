import { Dispatch, FC, Fragment, SetStateAction } from "react";
import { TextField } from "@mui/material";
import { useForm } from "react-hook-form";

interface RendertargetInputProps {
  rendertargetName: string | undefined;
  setRendertargetName: Dispatch<SetStateAction<string | undefined>>;
  modelName: string | undefined;
  setModelName: Dispatch<SetStateAction<string | undefined>>;
}

export const RendertargetInput: FC<RendertargetInputProps> = (props) => {
  const {
    register,
    formState: { errors },
  } = useForm();

  return (
    <Fragment>
      <TextField
        id="standard-basic"
        {...register("rendertargetName", { required: true })}
        value={props.rendertargetName}
        error={!!errors.rendertargetName}
        helperText={errors.rendertargetName ? "Name is required" : undefined}
        onChange={(ev) => props.setRendertargetName(ev.target.value)}
        label="Rendertarget Name"
        variant="standard"
        sx={{ marginRight: "10px" }}
      />
      <TextField
        {...register("modelName", { required: true })}
        value={props.modelName}
        error={!!errors.modelName}
        helperText={errors.modelName ? "Name is required" : undefined}
        onChange={(ev) => props.setModelName(ev.target.value)}
        label="Model Name"
        variant="standard"
      />
    </Fragment>
  );
};
