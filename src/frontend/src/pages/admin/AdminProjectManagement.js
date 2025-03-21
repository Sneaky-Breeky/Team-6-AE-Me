import React, { useState, useEffect } from 'react';
import { Typography, Input, Form, Button, Tag, Flex, message, Spin, Popconfirm } from "antd"
import Box from '@mui/material/Box';
import { CloseOutlined, SearchOutlined, DeleteOutlined, QuestionCircleOutlined } from '@ant-design/icons';
import CreateNewFolderOutlinedIcon from '@mui/icons-material/CreateNewFolderOutlined';
import FolderDeleteOutlinedIcon from '@mui/icons-material/FolderDeleteOutlined';
import dayjs from 'dayjs';
import { projects, files, logs } from '../../utils/dummyData.js';
import {fetchProjects, postProject} from '../../api/projectApi';

const { Title } = Typography;

const tagStyle = {
    backgroundColor: '#dbdbdb'
};

export default function ProjectManagement() {
    const [createOpen, setCreateOpen] = useState(false);
    const [deleteOpen, setDeleteOpen] = useState(false);
    const [searchQuery, setSearchQuery] = useState('');
    const [loading, setLoading] = useState(true);
    const [fetchedProjects, setFetchedProjects] = useState([]);
    const [messageApi, contextHolder] = message.useMessage();
    const [form] = Form.useForm();


    // Fetch projects
    const getProjects = async () => {
        setLoading(true);
        const response = await fetchProjects();

        if (response.error) {
            console.error("Error fetching projects:", response.error);
            setFetchedProjects([]);
        } else {
            console.log("Fetched Projects:", response);
            setFetchedProjects(response);
        }

        setLoading(false);
    };

    useEffect(() => {
        getProjects();
    }, []);


    const handleAddProjectButton = async (values) => {
        console.log("Submitting data:", values);

        const projectData = {
            name: values.projectName,
            description: values.description,
            status: "Active",
            location: values.location || "",
            imagePath: null,
            phase: "",
            accessLevel: 0,
            lastUpdated: new Date().toISOString(),
            files: [],
            users: []
        };

        try {
            const result = await postProject(projectData);

            if (result.error) {
                throw new Error(result.error);
            }

            console.log("Project successfully added:", result);
            message.success("Project added successfully");
            form.resetFields();

            await getProjects();

        } catch (error) {
            console.error("Error adding project:", error);
            message.error("Failed to add project");
        }
    };

    const handleDeleteProject = async (p) => {
        console.log(p);
        console.log(p.id);

        /*  try {
            const response = await fetch(`http://localhost:5146/api/Projects/${p.id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
    
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
    
            const result = await response.json();
            console.log("Project successfully deleted:", result);
            message.success("Project deleted successfully");

        } catch (error) {
            console.error("Error deleting project:", error);
            message.error("Failed to delete project");
        }*/

       
    };
    



    // const handleAddProjectButton = (values) => {
    //     // send project info to backend here
    //     console.log("Submitting data:", values);

    //     const projectData = {
    //         ...values, // projectName, description, location
    //         metadataFields: defaultMetadataFields,
    //         metadataTags: defaultMetadataTags,
    //     };

    //     const project = {
    //         id: projects.length,
    //         name: values.projectName, 
    //         location: values.location, 
    //         date: dayjs(),
    //         thumbnail: null, 
    //         accessLevel: 'Admins Only', 
    //         listUsers: [], 
    //         status: null,
    //         phase: null,
    //         lastUpdated: dayjs(),
    //         files: files
    //     };

    //     const projectFile = {
    //         Id: files.length, 
    //         FileName: project.name, 
    //         FilePath: null, 
    //         Metadata: defaultMetadataTags, 
    //         ProjectId: project.id, 
    //         Status: "Active", 
    //         Date: dayjs() 
    //    };

    //    const projectLog = {
    //         time: dayjs(), 
    //         action: 'Project created' 
    //    };

    //    projects.push(project);
    //    files.push(projectFile);
    //    logs.push(projectLog);
    //     //

    //     console.log("Final Project Data:", projects.at(-1));
    //     success();

    //     form.resetFields();
    //     setDefaultMetadataFields([]);
    //     setDefaultMetadataTags([]);
    // };

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                height: '100vh',
            }}
        >
            {/* Title */}
            <Box
                sx={{
                    textAlign: 'center',
                    padding: 4,
                }}
            >
                <Title level={1}>Manage Project Directories</Title>
            </Box>

            {/* Main content */}
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    flexGrow: 1,
                }}
            >

                {/* Container with create or delete projects */}
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'flex-start',
                        alignItems: 'left',
                        width: '20%',
                        height: "fit-content",
                        margin: '20px auto',
                        marginLeft: '10%',
                        marginRight: '0',
                        overflow: 'hidden',
                    }}
                >
                
                    {/*Create Project*/}
                    <Box
                        onClick={() => {
                            setDeleteOpen(false);
                            setCreateOpen(!createOpen);
                            console.log("create");
                        }}
                        sx={{
                        textAlign: 'center',
                        width: '80%',
                        height: '100%',
                        margin: '20px auto',
                        backgroundColor: 'grey.300',
                        border: 1,
                        borderColor: 'grey.500',
                        borderRadius: '16px',
                        '&:hover': { boxShadow: 3 },
                        }}
                    >
                        {!createOpen ? 
                            <CreateNewFolderOutlinedIcon style={{ marginTop: '10%',  marginBottom: '0', fontSize: '500%' }} />
                            : <CloseOutlined style={{ marginTop: '10%',  marginBottom: '0', fontSize: '500%' }} />
                        }
                        <h4  style={{ margin: '0', marginBottom: '10%'}}>{!createOpen ? "Create Project" : "Close"}</h4>
                    </Box>

                    {/*Delete Project*/}
                    <Box
                        onClick={() => {
                            setCreateOpen(false);
                            setDeleteOpen(!deleteOpen);
                            console.log("delete");
                        }}
                        sx={{
                        textAlign: 'center',
                        width: '80%',
                        height: '100%',
                        margin: '20px auto',
                        backgroundColor: 'grey.300',
                        border: 1,
                        borderColor: 'grey.500',
                        borderRadius: '16px',
                        '&:hover': { boxShadow: 3 },
                        }}
                    >
                        {!deleteOpen ? 
                            <FolderDeleteOutlinedIcon style={{ marginTop: '10%', marginBottom: '0', fontSize: '500%' }} />
                            : <CloseOutlined style={{ marginTop: '10%',  marginBottom: '0', fontSize: '500%' }} />
                        }
                        <h4 style={{ margin: '0', marginBottom: '10%'}}>{!deleteOpen ? "Delete Project" : "Close"}</h4>
                    </Box> 

                </Box>

                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'row',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        width: '60%',
                        height: "fit-content",
                        margin: '0',
                        overflow: 'hidden',
                    }}
                >

                {/* Container with inputs */}
                {createOpen && 
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'flex-start',
                        alignItems: 'left',
                        width: '80%',
                        height: "fit-content",
                        margin: '20px auto',
                        backgroundColor: '#f5f5f5',
                        borderRadius: '10px',
                        padding: '20px',
                        boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
                        overflow: 'hidden',
                    }}
                >
                    <Title level={4} style={{textAlign: 'center', marginTop: '0'}}>Create Directory</Title>
                    <Form
                        form={form}
                        name="project_creation"
                        layout="vertical"
                        autoComplete="off"
                        onFinish={handleAddProjectButton}
                        onKeyDown={(e) => {
                            if (e.key === "Enter") {
                                e.preventDefault();
                            }
                        }}
                    >
                        <Title level={5} style={{ marginTop: '10px' }}>
                            Project Name <span style={{ color: 'red' }}>*</span>
                        </Title>
                        <Form.Item
                            name="projectName"
                            rules={[{ required: true, message: "Please enter a project name" }]}
                        >
                            <Input placeholder="Enter project name" />
                        </Form.Item>

                        <Title level={5} style={{ marginTop: '10px' }}>
                            Description <span style={{ color: 'red' }}>*</span>
                        </Title>
                        <Form.Item
                            name="description"
                            rules={[{ required: true, message: "Please enter a description" }]}
                        >
                            <Input placeholder="Enter description" />
                        </Form.Item>

                        <Title level={5} style={{ marginTop: '10px' }}>Location</Title>
                        <Form.Item name="location">
                            <Input placeholder="Enter location" />
                        </Form.Item>
                        {contextHolder}

                        <div style={{ textAlign: "center", marginTop: "10px" }}>
                            <Button
                                htmlType="submit"
                                style={{
                                    marginTop: "10px",
                                    padding: "10px 20px",
                                    backgroundColor: "#00bcd4",
                                    borderColor: "#00bcd4",
                                    color: "white",
                                    fontWeight: "bold",
                                }}
                                type="primary"
                            >
                                Add Project
                            </Button>
                        </div>
                    </Form>
                </Box>}

                {deleteOpen &&
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'flex-start',
                        alignItems: 'left',
                        width: '80%',
                        height: "60vh",
                        margin: '20px auto',
                        backgroundColor: '#f5f5f5',
                        borderRadius: '10px',
                        padding: '20px',
                        boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
                        overflow: 'auto',
                    }}
                >
                    <Title level={4} style={{textAlign: 'center', marginTop: '0'}}>Delete Directory</Title>

                    <Input
                    placeholder="Search for a project.."
                    prefix={<SearchOutlined />}
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    style={{ width: '50%', marginBottom: '2%'}}
                    disabled={loading}
                    />

                    <div style={{overflowY: 'auto', width: '100%', height: '100%'}}>
                        {loading ? (
                            <div style={{ textAlign: 'center', padding: '20px' }}>
                                <Spin size="large" />
                                <p>Loading projects...</p>
                            </div>
                        ) : (
                            <table style={{ width: '100%', borderCollapse: 'collapse', borderTop: '1px solid black'}}>
                                {(fetchedProjects.filter((p) => p.name.toLowerCase().includes(searchQuery.toLowerCase()))).map((p) => (
                                    <tr
                                        key={p.id}
                                        
                                        style={{ height: '50px' }}
                                    >
                                        <td style={{ fontSize: '12px', textAlign: 'left', borderBottom: '1px solid black' }}>
                                            {p.id} <span style={{ color: 'gray', fontStyle: 'italic' }}> - {p.name}</span>
                                        </td>


                                        <td style={{ fontSize: '12px', textAlign: 'center', borderBottom: '1px solid black' }}>
                                        <Popconfirm
                                            title="Delete Project Directory"
                                            description="Are you sure you want to delete the selected project directory?"
                                            onConfirm={() => handleDeleteProject(p)}
                                            icon={<QuestionCircleOutlined style={{ color: 'red' }} />}
                                            okText="Yes"
                                            cancelText="No"
                                        >
                                            <Button type="primary" danger icon={<DeleteOutlined />}>
                                                Delete
                                            </Button>
                                        </Popconfirm>
                                        </td>
                                    </tr>
                                ))}
                            </table>
                        )}
                    </div>
                    
                </Box>}

                </Box>
            </Box>
        </Box>
    )
}