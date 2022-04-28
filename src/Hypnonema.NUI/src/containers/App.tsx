import {FC, useRef} from "react";
import Paper from "@mui/material/Paper";
import styled from "styled-components";
import {Header} from "../components/Header";
import Draggable from "react-draggable";
import {Outlet, useNavigate} from "react-router-dom";
import Menu from "../components/Menu";
import Box from "@mui/material/Box";
import {useAppDispatch, useAppSelector} from "../hooks/store";
import {useNuiEvent} from "fivem-nui-react-lib";
import {setIsAppVisible, setSnackbar, SnackbarSeverity} from "../reducers/ui";
import {setScreens} from "../reducers/screens";
import Screen from 'types/screen';
import {setDuiStates} from "../reducers/dui";
import {DuiState} from "../types/duiState";
import {Snackbar} from "../components/Snackbar";

const Wrapper = styled.div`
  width: 900px;
  height: 600px;
  margin: auto;
`;

export const App: FC = () => {
    const nodeRef = useRef(null);
    const paperRef = useRef(null);

    const dispatch = useAppDispatch();

    const resourceName = 'hypnonema';

    const navigate = useNavigate();

    useNuiEvent<boolean>(resourceName, 'showUI', r => dispatch(setIsAppVisible(r)));

    useNuiEvent<Screen[]>(resourceName, 'getScreenList', r => dispatch(setScreens(r)));

    useNuiEvent<DuiState[]>(resourceName, 'requestState', r => onDuiStates(r));

    useNuiEvent<Screen>(resourceName, 'createScreen', r => onCreatedScreen(r));

    useNuiEvent<string>(resourceName, 'deleteScreen', r => onDeletedScreen(r));

    useNuiEvent<Screen>(resourceName, 'editScreen', r => onEditedScreen(r));

    const showSnackbar = (message: string, type: SnackbarSeverity = SnackbarSeverity.Success) => {
        dispatch(setSnackbar({
            isVisible: true,
            type,
            message
        }))
    }

    const onDuiStates = (duiStates: DuiState[]) => {
        dispatch(setDuiStates(duiStates));
    }

    const onEditedScreen = (screen: Screen) => {
        showSnackbar(`Screen "${screen.name}" successfully updated!`);
    }

    const onDeletedScreen = (screenName: string) => {
        showSnackbar(`Screen "${screenName}" successfully deleted!`);
    }

    const onCreatedScreen = (screen: Screen) => {
        navigate('/screens', {replace: true});
        showSnackbar(`Screen "${screen.name}" successfully created!`);
    }

    const isAppVisible = useAppSelector((state => state.ui.isAppVisible))

    if (!isAppVisible) {
        return null;
    }

    return (
        <Draggable
            handle="#appHeader"
            nodeRef={nodeRef}>
            <Wrapper
                ref={nodeRef}
                style={{
                    marginTop: '80px'
                }}>
                <Paper
                    ref={paperRef}
                    sx={{
                        height: 'inherit',
                        width: 'inherit',
                        backgroundImage: 'none',
                        backgroundColor: '#303030 !important'
                    }}
                    elevation={6}
                >
                    <Box
                        sx={{
                            display: 'flex',
                            height: 'inherit'
                        }}>
                        <Header/>
                        <Menu containerRef={paperRef}/>
                        <Box
                            component="main"
                            sx={{
                                flexGrow: 1,
                                p: 3,
                                maxHeight: '600px',
                            }}>
                            <Outlet/>
                        </Box>
                    </Box>
                    <Snackbar/>
                </Paper>
            </Wrapper>
        </Draggable>
    );
}
