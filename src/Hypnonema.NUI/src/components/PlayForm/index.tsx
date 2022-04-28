import { Button, Card, CardContent, Container, TextField } from "@mui/material";
import { ScreenSelect } from "../ScreenSelect";
import styled from "styled-components";
import { FC, useState } from "react";
import Screen from "types/screen";
import { useForm } from "react-hook-form";

const Wrapper = styled.div`
  margin-top: 30px;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
`;

interface PlayFormProps {
  screens: Screen[];
  onSubmit: Function;
}

export const PlayForm: FC<PlayFormProps> = (props) => {
  const [screen, setScreen] = useState("");

  const [url, setUrl] = useState("");

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = (data: any) => {
    props.onSubmit(screen, data.url);
  };

  return (
    <Wrapper>
      <Card
        sx={{
          width: "100%",
          maxWidth: "100%",
          boxShadow: "6",
        }}
      >
        <CardContent>
          <Container>
            <form onSubmit={handleSubmit((data: any) => onSubmit(data))}>
              <ScreenSelect
                screens={props.screens}
                onChange={(selected: string) => setScreen(selected)}
              />
              <TextField
                {...register("url", {
                  required: true,
                  validate: (value) => {
                    try {
                      new URL(value);
                      return true;
                    } catch (e) {
                      return false;
                    }
                  },
                })}
                value={url}
                fullWidth
                required
                error={!!errors.url}
                helperText={errors.url ? "URL is invalid!" : undefined}
                onChange={(ev) => setUrl(ev.target.value)}
                label="URL"
                sx={{ marginTop: "25px" }}
              />
              <div>
                <Button
                  sx={{
                    marginTop: "2rem",
                    float: "right",
                    marginBottom: "1rem",
                  }}
                  type="submit"
                  variant="contained"
                >
                  Play
                </Button>
              </div>
            </form>
          </Container>
        </CardContent>
      </Card>
    </Wrapper>
  );
};
