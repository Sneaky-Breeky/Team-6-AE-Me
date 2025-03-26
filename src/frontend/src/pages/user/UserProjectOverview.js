import React, { useState } from 'react';
import Box from '@mui/material/Box';
import { Input, Button, DatePicker, Form, Typography, Card, Row, Col, Select, Space, Image, Popconfirm, Tooltip } from 'antd';
import {
    SearchOutlined,
    CalendarOutlined,
    DownloadOutlined,
    LeftOutlined,
    RightOutlined,
    RotateLeftOutlined,
    RotateRightOutlined,
    SwapOutlined,
    UndoOutlined,
    ZoomInOutlined,
    ZoomOutOutlined,
    EditOutlined,
    QuestionCircleOutlined
} from '@ant-design/icons';
import { useNavigate, useLocation, useParams } from 'react-router-dom';
import { projects, users } from '../../utils/dummyData.js';
import dayjs from 'dayjs';

const { RangePicker } = DatePicker;
const { Title } = Typography;
const { Meta } = Card;

// instead of passing project via state, try access the url id and using params
export default function UserProjectOverview() {
    const { projectId } = useParams();
    const project = projects.find(proj => proj.id === projectId);
    const [searchQuery, setSearchQuery] = useState('');
    const [selectedDate, setSelectedDate] = useState(null);
    const navigate = useNavigate();
    const [current, setCurrent] = React.useState(0);
    const { state } = useLocation();
    const [imageList, setImageList] = useState(state.project.files);
    const [isEditMode, setIsEditMode] = useState(false);

    const [selectedImages, setSelectedImages] = useState(new Set());
    const [selectedStatus, setSelectedStatus] = useState("");



    if (!state?.project) {
        return <p>Project not found.</p>;
    }

    // if (!project) {
    //     return <p>Project not found.</p>;
    // }


    // when backend is done connect this part with backend
    const handleSearch = () => {
        let filteredImages = state.project.files;

        console.log(state.project.status);
        if (searchQuery.trim() !== '') {
            const query = searchQuery.toLowerCase();
            filteredImages = filteredImages.filter(file =>
                file.Metadata.some(tag => tag.toLowerCase().includes(query))
            );
        }

        if (selectedDate) {
            filteredImages = filteredImages.filter(file =>
                dayjs(file.Date).isSame(selectedDate, 'day')
            );
        }

        if (selectedStatus) {
            filteredImages = filteredImages.filter(file =>
                file.Status.toLowerCase() === selectedStatus.toLowerCase()
            );
        }

        setImageList(filteredImages);
        console.log("Filtered images:", filteredImages);
    };


    const handleClearFilters = () => {
        setSearchQuery('');
        setSelectedDate(null);
        setSelectedStatus('');
        setImageList(state.project.files);
    };


    const onDownload = () => {
        const url = imageList[current];
        const suffix = url.slice(url.lastIndexOf('.'));
        const filename = Date.now() + suffix;

        fetch(url)
            .then((response) => response.blob())
            .then((blob) => {
                const blobUrl = URL.createObjectURL(new Blob([blob]));
                const link = document.createElement('a');
                link.href = blobUrl;
                link.download = filename;
                document.body.appendChild(link);
                link.click();
                URL.revokeObjectURL(blobUrl);
                link.remove();
            });
    };

    const toggleEditMode = () => {
        setIsEditMode((prev) => !prev);
        setSelectedImages(new Set());
    };

    const toggleSelectImage = (index) => {
        if (!isEditMode) return;

        const updatedSelection = new Set(selectedImages);
        if (updatedSelection.has(index)) {
            updatedSelection.delete(index);
        } else {
            updatedSelection.add(index);
        }
        setSelectedImages(updatedSelection);
    };

    const downloadSelectedImages = () => {
        console.log(selectedImages);
        //setImageList(imageList.filter((_, index) => !selectedImages.has(index)));
        setSelectedImages(new Set());
        setIsEditMode(false);
      };

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                height: '100vh',
            }}
        >
            <Box
                sx={{
                    position: 'relative',
                    width: '100%',
                    padding: 6,
                    textAlign: 'center',
                }}
            >
                {/* Project info card at the top left */}
                <Box
                    sx={{
                        position: 'absolute',
                        left: 0,
                        top: 0,
                        marginLeft: '40px',
                        marginTop: '20px'
                    }}
                >
                    <Card
                        style={{
                            backgroundColor: '#37474F',
                            borderRadius: '10px',
                            overflow: 'hidden',
                            width: '250px',
                            color: 'white',
                            boxShadow: '2px 4px 10px rgba(0, 0, 0, 0.2)'
                        }}
                    >
                        <Meta
                            title={<span style={{ color: 'white', fontWeight: 'bold' }}>{state.project.name}</span>}
                            description={<span style={{ color: 'white' }}>{state.project.location}</span>}
                            style={{ textAlign: 'center' }}
                        />
                    </Card>
                </Box>

                {/* Title */}
                <Title level={1} style={{ margin: '0 auto', textAlign: 'center' }}>Project Overview</Title>
            </Box>




            {/* Search stuff*/}
            <Box
                sx={{
                    flexGrow: 1,
                    backgroundColor: 'gray.50',
                    padding: 1,
                }}
            >
                <Form
                    layout="inline"
                    onFinish={handleSearch}
                    style={{
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        gap: '10px',
                    }}
                >
                    <Form.Item>
                        <Input
                            placeholder="Search images by key word..."
                            prefix={<SearchOutlined />}
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            style={{ width: '300px' }}
                        />
                    </Form.Item>

                    <Form.Item>
                        <Select
                            defaultValue="Active"
                            style={{ width: 120 }}
                            allowClear
                            options={[
                                { value: 'active', label: 'Active' },
                                { value: 'archived', label: 'Archived' }]}
                            onChange={(value) => setSelectedStatus(value)}
                            placeholder="Select status"
                        />
                    </Form.Item>

                    <Form.Item>
                        <DatePicker
                            maxDate={dayjs()}
                            placeholder="Select date"
                            onChange={(date, dateString) => setSelectedDate(dateString)}
                            suffixIcon={<CalendarOutlined />}
                        />
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" color="cyan" variant="solid">
                            Search
                        </Button>
                    </Form.Item>

                    <Form.Item>
                        <Button type="default" onClick={handleClearFilters} danger>
                            Clear Filters
                        </Button>
                    </Form.Item>
                </Form>
            </Box>



            {/* Main content */}
            <Box sx={{ display: 'flex', flexDirection: 'column', height: '100vh' }}>
                {/* Project Metadata */}
                <Box sx={{ 
                    display: 'flex',
                    flexDirection: 'column', 
                    justifyContent: 'space-between',
                    alignItems: 'start',
                    width:'70%',
                    margin: '20px auto',
                    backgroundColor: '#f5f5f5',
                    borderRadius: '10px',
                    padding: '20px',
                    boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)' }}>
                        <div style={{display: 'flex', flexDirection: 'row', justifyContent: 'space-between', width:'100%'}}>
                        <div><strong>Location: </strong><span>{state.project.location}</span></div>
                        <div><strong>Start Date: </strong><span>{dayjs(state.project.startDate).format('MMM DD, YYYY')}</span></div>
                        <div><strong>State: </strong><span>{state.project.status}</span></div>
                        <div><strong>Phase: </strong><span>{state.project.phase}</span></div>
                        </div>
                        <div style={{margin: '20px auto', marginBottom: '0', width:'100%'}}><strong>Metadata: </strong>{
                            state.project.fields.map((f) => (
                                <span>{f.field}: <span style={{ color: 'grey', fontStyle: 'italic' }}>{f.fieldMD} </span></span>
                            ))
                        }</div>
                </Box>
                {/* Download Images */}
                <Box sx={{ display: 'flex', alignItems: 'start', justifyContent: 'left', margin: '20px auto', gap: '10px', width:'70%', }}>
                <Button
                    onClick={toggleEditMode}
                    color="primary" 
                    type="primary"
                    ghost={isEditMode}
                    icon={<EditOutlined />}
                >
                    {isEditMode ? "Cancel Selection Mode" : "Select from Gallery"}
                </Button>
                {/* Download button */}
                {isEditMode && selectedImages.size > 0 && (
                    <Popconfirm
                        title="Download Images"
                        description="Are you sure you want to download the selected images?"
                        onConfirm={downloadSelectedImages}
                        icon={<QuestionCircleOutlined style={{ color: 'gray' }} />}
                        okText="Yes"
                        cancelText="No"
                    >
                        <Button color="primary" variant="filled" icon={<DownloadOutlined />}>
                            Download
                        </Button>
                    </Popconfirm>
                )}
                </Box>
                {/* Image gallery*/}
                <Box sx={{ display: 'flex', justifyContent: 'center', padding: 2 }}>
                {isEditMode ? (
                        <Space wrap size={16} style={{ justifyContent: 'center' }}>
                            {imageList.map((file) => (
                                <div
                                    key={file.Id}
                                    style={{ position: 'relative', cursor: 'pointer' }}
                                    onClick={() => toggleSelectImage(file.Id)}
                                >
                                    <Image
                                        src={file.FilePath}
                                        width={200}
                                        preview={false}
                                        style={{
                                            border: selectedImages.has(file.Id) ? '4px solid blue' : 'none',
                                            borderRadius: '8px',
                                            transition: '0.2s ease-in-out',
                                        }}
                                    />
                                    {selectedImages.has(file.Id) && (
                                        <DownloadOutlined
                                            style={{
                                                position: 'absolute',
                                                top: 5,
                                                right: 5,
                                                color: 'white',
                                                background: 'blue',
                                                borderRadius: '50%',
                                                padding: '5px',
                                                cursor: 'pointer',
                                                fontSize: '16px',
                                            }}
                                        />
                                    )}
                                </div>
                            ))}

                        </Space>
                    ) : (
                        <Image.PreviewGroup
                            preview={{
                                toolbarRender: (_, { transform: { scale }, actions }) => (
                                    <Space size={12} className="toolbar-wrapper">
                                        <LeftOutlined onClick={() => actions.onActive?.(-1)} />
                                        <RightOutlined onClick={() => actions.onActive?.(1)} />
                                        <DownloadOutlined onClick={onDownload} />
                                        <SwapOutlined rotate={90} onClick={actions.onFlipY} />
                                        <SwapOutlined onClick={actions.onFlipX} />
                                        <RotateLeftOutlined onClick={actions.onRotateLeft} />
                                        <RotateRightOutlined onClick={actions.onRotateRight} />
                                        <ZoomOutOutlined disabled={scale === 1} onClick={actions.onZoomOut} />
                                        <ZoomInOutlined disabled={scale === 50} onClick={actions.onZoomIn} />
                                        <UndoOutlined onClick={actions.onReset} />
                                    </Space>
                                ),
                                onChange: (index) => setCurrent(index),
                            }}
                        >
                            <Space wrap size={16} style={{ justifyContent: 'center' }}>

                                {imageList.map((file) => (
                                    <Tooltip title={
                                        <>{file.Metadata.map((md, index) => 
                                            index < file.Metadata.length - 1 ? (
                                                <span key={index}>{md}, </span>
                                            ) : (
                                                <span key={index}>{md}</span>
                                            )
                                        )}</>}>
                                        <Image
                                        key={file.Id}
                                        src={file.FilePath}
                                        width={200}
                                    />
                                    </Tooltip>
                                ))}
                            </Space>
                        </Image.PreviewGroup>
                    )}
                </Box>
            </Box>
        </Box>
    );
}