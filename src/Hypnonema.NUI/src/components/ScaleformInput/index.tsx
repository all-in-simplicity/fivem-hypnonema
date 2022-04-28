import { Dispatch, FC, Fragment, SetStateAction } from "react";
import { TextField } from "@mui/material";

const marginTop = "15px";
const marginRight = "6px";

interface ScaleformInputProps {
  positionX: number | undefined;
  setPositionX: Dispatch<SetStateAction<number | undefined>>;
  positionY: number | undefined;
  setPositionY: Dispatch<SetStateAction<number | undefined>>;
  positionZ: number | undefined;
  setPositionZ: Dispatch<SetStateAction<number | undefined>>;
  scaleX: number | undefined;
  setScaleX: Dispatch<SetStateAction<number | undefined>>;
  scaleY: number | undefined;
  setScaleY: Dispatch<SetStateAction<number | undefined>>;
  scaleZ: number | undefined;
  setScaleZ: Dispatch<SetStateAction<number | undefined>>;
  rotationX: number | undefined;
  setRotationX: Dispatch<SetStateAction<number | undefined>>;
  rotationY: number | undefined;
  setRotationY: Dispatch<SetStateAction<number | undefined>>;
  rotationZ: number | undefined;
  setRotationZ: Dispatch<SetStateAction<number | undefined>>;
}

export const ScaleformInput: FC<ScaleformInputProps> = (props) => {
  return (
    <Fragment>
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Position X"
        value={props.positionX}
        onChange={(ev) => props.setPositionX(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Position Y"
        value={props.positionY}
        onChange={(ev) => props.setPositionY(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Position Z"
        value={props.positionZ}
        onChange={(ev) => props.setPositionZ(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Rotation X"
        value={props.rotationX}
        onChange={(ev) => props.setRotationX(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Rotation Y"
        value={props.rotationY}
        onChange={(ev) => props.setRotationY(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Rotation Z"
        value={props.rotationZ}
        onChange={(ev) => props.setRotationZ(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Scale X"
        value={props.scaleX}
        onChange={(ev) => props.setScaleX(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Scale Y"
        value={props.scaleY}
        onChange={(ev) => props.setScaleY(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
      <TextField
        sx={{
          marginTop: marginTop,
          marginRight: marginRight,
        }}
        label="Scale Z"
        value={props.scaleZ}
        onChange={(ev) => props.setScaleZ(parseFloat(ev.target.value))}
        type="number"
        InputLabelProps={{
          shrink: true,
        }}
      />
    </Fragment>
  );
};
