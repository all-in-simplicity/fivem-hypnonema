import { FC, Fragment, useState } from "react";
import styled from "styled-components";
import {
  Button,
  Card,
  CardContent,
  Checkbox,
  Container,
  FormControlLabel,
  FormGroup,
  Stack,
  TextField,
} from "@mui/material";
import Screen from "types/screen";
import SimpleBar from "simplebar-react";

import { useForm } from "react-hook-form";
import { ScaleformInput } from "../ScaleformInput";

const Wrapper = styled.div`
  margin-top: 30px;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
`;

const marginTop = "15px";

interface ScreenFormProps {
  screen?: Screen;
  onSubmit: Function;
}

export const ScreenForm: FC<ScreenFormProps> = (props) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const [screenName, setScreenName] = useState(props.screen?.name);

  const [alwaysOn, setAlwaysOn] = useState(props.screen?.alwaysOn);

  const [is3DRendered, setIs3DRendered] = useState(props.screen?.is3DRendered);
  const [positionX, setPositionX] = useState(
    props.screen?.positionalSettings?.positionX
  );
  const [positionY, setPositionY] = useState(
    props.screen?.positionalSettings?.positionY
  );
  const [positionZ, setPositionZ] = useState(
    props.screen?.positionalSettings?.positionZ
  );
  const [rotationX, setRotationX] = useState(
    props.screen?.positionalSettings?.rotationX
  );
  const [rotationY, setRotationY] = useState(
    props.screen?.positionalSettings?.rotationY
  );
  const [rotationZ, setRotationZ] = useState(
    props.screen?.positionalSettings?.rotationZ
  );
  const [scaleX, setScaleX] = useState(
    props.screen?.positionalSettings?.scaleX
  );
  const [scaleY, setScaleY] = useState(
    props.screen?.positionalSettings?.scaleY
  );
  const [scaleZ, setScaleZ] = useState(
    props.screen?.positionalSettings?.scaleZ
  );
  const [soundMinDistance, setSoundMinDistance] = useState(
    props.screen?.browserSettings?.soundMinDistance
  );
  const [soundMaxDistance, setSoundMaxDistance] = useState(
    props.screen?.browserSettings?.soundMaxDistance
  );
  const [soundAttenuation, setSoundAttenuation] = useState(
    props.screen?.browserSettings?.soundAttenuation
  );
  const [modelName, setModelName] = useState(
    props.screen?.targetSettings?.modelName
  );
  const [renderTargetName, setRenderTargetName] = useState(
    props.screen?.targetSettings?.renderTargetName
  );

  const onSubmit = (data: any) => {
    const screen: Screen = {
      id: props.screen?.id!,
      name: data.screenName,
      alwaysOn: data.alwaysOn,
      browserSettings: {
        is3DAudioEnabled: false,
        globalVolume: 100, // TODO: Remove
        soundMinDistance: data.soundMinDistance,
        soundMaxDistance: data.soundMaxDistance,
        soundAttenuation: data.soundAttenuation,
      },
      is3DRendered: data.is3DRendered,
      positionalSettings: data.is3DRendered
        ? {
            rotationX: rotationX,
            rotationY: rotationY,
            rotationZ: rotationZ,
            scaleX: scaleX,
            scaleY: scaleY,
            scaleZ: scaleZ,
            positionX: positionX,
            positionY: positionY,
            positionZ: positionZ,
          }
        : {},
      targetSettings: data.is3DRendered
        ? {}
        : {
            renderTargetName: data.renderTargetName,
            modelName: data.modelName,
          },
    };
    props.onSubmit(screen);
  };

  return (
    <div>
      <Wrapper>
        <Card
          sx={{
            width: "100%",
            maxWidth: "100%",
            boxShadow: "6",
          }}
        >
          <SimpleBar
            autoHide={false}
            style={{
              maxHeight: "420px",
            }}
          >
            <CardContent>
              <Container>
                <form onSubmit={handleSubmit((data: any) => onSubmit(data))}>
                  <Stack direction="row" spacing={4}>
                    <TextField
                      id="standard-basic"
                      {...register("screenName", { required: true })}
                      value={screenName}
                      error={!!errors.screenName}
                      helperText={
                        errors.screenName ? "Name is required" : undefined
                      }
                      onChange={(ev) => setScreenName(ev.target.value)}
                      label="Name"
                      variant="standard"
                    />
                    <FormGroup
                      sx={{
                        marginLeft: "auto",
                        marginTop: "18px !important",
                      }}
                    >
                      <FormControlLabel
                        control={
                          <Checkbox
                            {...register("alwaysOn")}
                            checked={alwaysOn}
                            onChange={(ev) => setAlwaysOn(ev.target.checked)}
                          />
                        }
                        label="Always On"
                      />
                    </FormGroup>
                  </Stack>
                  <FormGroup
                    sx={{
                      marginTop: "18px !important",
                      display: "inline-block !important",
                      marginLeft: "auto",
                    }}
                  >
                    <FormControlLabel
                      control={
                        <Checkbox
                          {...register("is3DRendered")}
                          checked={is3DRendered}
                          onChange={(ev) => setIs3DRendered(ev.target.checked)}
                        />
                      }
                      label="Is 3D Rendered"
                    />
                  </FormGroup>
                  <br />
                  {!is3DRendered && (
                    <Fragment>
                      <TextField
                        {...register("renderTargetName", { required: true })}
                        value={renderTargetName}
                        error={!!errors.renderTargetName}
                        helperText={
                          errors.renderTargetName
                            ? "RenderTarget Name is required"
                            : undefined
                        }
                        onChange={(ev) => setRenderTargetName(ev.target.value)}
                        label="RenderTarget Name"
                        variant="standard"
                        sx={{ marginRight: "10px" }}
                      />
                      <TextField
                        {...register("modelName", { required: true })}
                        value={modelName}
                        error={!!errors.modelName}
                        helperText={
                          errors.modelName
                            ? "Model Name is required"
                            : undefined
                        }
                        onChange={(ev) => setModelName(ev.target.value)}
                        label="Model Name"
                        variant="standard"
                      />
                    </Fragment>
                  )}
                  {is3DRendered && (
                    <ScaleformInput
                      positionX={positionX}
                      setPositionX={setPositionX}
                      positionY={positionY}
                      setPositionY={setPositionY}
                      positionZ={positionZ}
                      setPositionZ={setPositionZ}
                      scaleX={scaleX}
                      setScaleX={setScaleX}
                      scaleY={scaleY}
                      setScaleY={setScaleY}
                      scaleZ={scaleZ}
                      setScaleZ={setScaleZ}
                      rotationX={rotationX}
                      setRotationX={setRotationX}
                      rotationY={rotationY}
                      setRotationY={setRotationY}
                      rotationZ={rotationZ}
                      setRotationZ={setRotationZ}
                    />
                  )}
                  <br />
                  <div style={{ marginTop: "20px" }}>
                    <TextField
                      {...register("soundMinDistance")}
                      sx={{
                        marginTop: marginTop,
                        marginRight: "6px",
                      }}
                      label="Sound Min. Distance"
                      value={soundMinDistance}
                      onChange={(ev) =>
                        setSoundMinDistance(parseInt(ev.target.value))
                      }
                      type="number"
                      InputLabelProps={{
                        shrink: true,
                      }}
                    />
                    <TextField
                      {...register("soundMaxDistance")}
                      sx={{
                        marginTop: marginTop,
                        marginRight: "6px",
                      }}
                      label="Sound Max. Distance"
                      value={soundMaxDistance}
                      onChange={(ev) =>
                        setSoundMaxDistance(parseInt(ev.target.value))
                      }
                      type="number"
                      InputLabelProps={{
                        shrink: true,
                      }}
                    />
                    <TextField
                      {...register("soundAttenuation")}
                      sx={{
                        marginTop: marginTop,
                      }}
                      label="Sound Attenuation"
                      value={soundAttenuation}
                      onChange={(ev) =>
                        setSoundAttenuation(parseInt(ev.target.value))
                      }
                      type="number"
                      InputLabelProps={{
                        shrink: true,
                      }}
                    />
                  </div>
                  <div
                    style={{
                      marginTop: marginTop,
                      minHeight: "30px",
                    }}
                  >
                    <Button
                      variant="contained"
                      type="submit"
                      sx={{
                        float: "right",
                        bottom: 0,
                        marginBottom: "5px",
                      }}
                    >
                      Submit
                    </Button>
                  </div>
                </form>
              </Container>
            </CardContent>
          </SimpleBar>
        </Card>
      </Wrapper>
    </div>
  );
};
