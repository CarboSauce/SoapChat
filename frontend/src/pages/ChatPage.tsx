import { useState } from 'react'
import { useAuth } from '../context/AuthContext'
import Avatar from '../components/Avatar'
import GroupsList from '../components/GroupsList'
import ChatView from '../components/ChatView'
import '../styles/chat.css'

export default function ChatPage() {
  const { user, logout } = useAuth()
  const [selectedGroupId, setSelectedGroupId] = useState<string | null>(null)
  const [avatarKey, setAvatarKey] = useState(0)

  return (
    <div className="chat-container">
      <div className="chat-header">
        <h1>SoapChat</h1>
        <div className="user-info">
          <Avatar 
            key={avatarKey}
            userId={user?.id || ''} 
            size="small" 
            editable={true}
            onUploadComplete={() => setAvatarKey(k => k + 1)}
          />
          <span>{user?.name || user?.username}</span>
          <button onClick={logout} className="btn btn-secondary">
            Logout
          </button>
        </div>
      </div>

      <div className="chat-content">
        <div className="groups-panel">
          <GroupsList 
            selectedGroupId={selectedGroupId}
            onSelectGroup={setSelectedGroupId}
          />
        </div>

        <div className="messages-panel">
          {selectedGroupId ? (
            <ChatView groupId={selectedGroupId} />
          ) : (
            <div className="no-group-selected">
              <p>Select a group to start chatting</p>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
